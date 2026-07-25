using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRenderer : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] private NavMeshSurface _surface;
    [SerializeField] private Transform _roomContentParent;
    [SerializeField] private List<Transform> _shopItemPoints;

    [Header("Doors")]
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _rightDoor;
    [SerializeField] private GameObject _upDoor;
    [SerializeField] private GameObject _downDoor;
    
    [Header("Spawn positions")]
    [SerializeField] private Transform _leftSpawn;
    [SerializeField] private Transform _rightSpawn;
    [SerializeField] private Transform _upSpawn;
    [SerializeField] private Transform _downSpawn;

    [Header("Enemies")]
    [SerializeField] private List<EnemyAI> _enemies;

    [Header("Chest")]
    [SerializeField] private LootChest _chestPrefab;
    [SerializeField] private List<Item> _chestItems = new();
    [SerializeField, Min(1)] private int _maxItemsInChest = 4;

    [Header("Shop")]
    [SerializeField] private List<Item> _shopItems = new();

    private List<EnemyAI> _spawnedEnemy = new();
    private bool _isRoomBlocked = false;
    protected Vector2Int PlayerRoom => Player.Instance.CurrentRoomCoords;

    private void Start()
    {
        StartCoroutine(RerenderRoom(PlayerRoom));
    }

    private IEnumerator RerenderRoom(Vector2Int roomPosition)
    {
        _isRoomBlocked = false;
        _spawnedEnemy.Clear();
        yield return null;

        foreach (Transform child in _roomContentParent.transform)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        if (!MapController.Instance.RoomExists(roomPosition))
            yield break;

        var room = MapController.Instance.GetRoom(roomPosition);

        _leftDoor.SetActive(false);
        _rightDoor.SetActive(false);
        _upDoor.SetActive(false);
        _downDoor.SetActive(false);

        if (MapController.Instance.RoomExists(roomPosition + Vector2Int.left))
            _leftDoor.SetActive(true);
        if (MapController.Instance.RoomExists(roomPosition + Vector2Int.right))
            _rightDoor.SetActive(true);
        if (MapController.Instance.RoomExists(roomPosition + Vector2Int.up))
            _upDoor.SetActive(true);
        if (MapController.Instance.RoomExists(roomPosition + Vector2Int.down))
            _downDoor.SetActive(true);
        yield return null;

        _surface.BuildNavMesh();
        yield return null;

        switch (room.State)
        {
            case RoomState.RoomType.Chest:
                if (room.IsCleared)
                    break;

                if (!room.IsLootGenerated && _chestItems.Count > 0)
                {
                    var itemsCount = Random.Range(1, _maxItemsInChest);

                    for (var i = 0; i < itemsCount; i++)
                    {
                        var itemIndex = Random.Range(0, _chestItems.Count);
                        var item = _chestItems[itemIndex];
                        room.LootAdd(item);
                    }
                }

                var chest = Instantiate(_chestPrefab, Vector3.zero, Quaternion.identity, _roomContentParent);
                chest.SetLoot(room.LootItems, _roomContentParent);
                chest.OnOpen += () => room.RoomClear();
                break;
            case RoomState.RoomType.Enemies:
                if (room.IsCleared || room.EnemiesCount <= 0)
                    break;

                CloseDoors();
                for (int i = 0; i < room.EnemiesCount; i++)
                {
                    var enemyIndex = Random.Range(0, _enemies.Count);
                    var enemyPrefab = _enemies[enemyIndex];

                    Vector3 position = Vector3.zero;
                    yield return Utils.GetRandomPointOnNavMesh(Vector3.zero, 20, point => position = point);

                    var enemy = Instantiate(enemyPrefab, position, Quaternion.identity, _roomContentParent);
                    enemy.OnDeath += EnemyDead;
                    _spawnedEnemy.Add(enemy);
                }
                break;
            case RoomState.RoomType.Shop:
                if (room.ShopItems.Count <= 0 && _shopItems.Count > 0)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var itemIndex = Random.Range(0, _shopItems.Count);
                        var item = _shopItems[itemIndex];
                        room.ShopItems.Add(new (item, i));
                    } 
                    room.RoomClear();
                }

                for (var i = 0; i < room.ShopItems.Count; i++)
                {
                    var shopItem = room.ShopItems[i];
                    if (shopItem.IsBuyed)
                        continue;

                    var position = _shopItemPoints[shopItem.Slot].position;
                    var itemSpawned = Instantiate(shopItem.ShopItem, position, Quaternion.identity, _roomContentParent);
                    itemSpawned.OnBuy += shopItem.Item_OnBuy;
                    itemSpawned.SourcePrefab = shopItem.ShopItem;
                    itemSpawned.SetAsShopItem();
                }
                break;
            case RoomState.RoomType.Empty:
                break;
            case RoomState.RoomType.Boss:
                break;
        }
        yield return null;

        foreach (var item in room.GroundLoot)
        {
            var itemSpawned = Instantiate(item.Item, item.Position, Quaternion.identity, _roomContentParent);
            itemSpawned.SourcePrefab = item.Item;
        }
    }

    private void EnemyDead(EnemyAI enemy)
    {
        _spawnedEnemy.Remove(enemy);

        if (_spawnedEnemy.Count == 0)
        {
            OpenDoors();
            MapController.Instance.GetRoom(PlayerRoom).RoomClear();
            StartCoroutine(RerenderRoom(PlayerRoom));
        }
    }

    private void CloseDoors()
    {
        _isRoomBlocked = true;
    }

    private void OpenDoors()
    {
        _isRoomBlocked = false;
    }

    private void MoveToRoom(Vector2Int direction)
    {
        if (_isRoomBlocked)
            return;

        var next = PlayerRoom + direction;
        if (!MapController.Instance.IsRoomPossible(next) || !MapController.Instance.IsRoom(next))
        {
            Debug.LogError("Player moved into impossible room");
            return;
        }

        if (direction == Vector2Int.left)
            Player.Instance.transform.position = _rightSpawn.position;
        else if (direction == Vector2Int.right)
            Player.Instance.transform.position = _leftSpawn.position;
        else if (direction == Vector2Int.up)
            Player.Instance.transform.position = _downSpawn.position;
        else if (direction == Vector2Int.down)
            Player.Instance.transform.position = _upSpawn.position;
        
        MapController.Instance.GetRoom(PlayerRoom).SaveGroundLoot(_roomContentParent);

        Player.Instance.EnterRoom(next);
        StartCoroutine(RerenderRoom(next));
    }

    public void MoveLeftRoom() => MoveToRoom(Vector2Int.left);
    public void MoveRightRoom() => MoveToRoom(Vector2Int.right);
    public void MoveUpRoom() => MoveToRoom(Vector2Int.up);
    public void MoveDownRoom() => MoveToRoom(Vector2Int.down);
}
