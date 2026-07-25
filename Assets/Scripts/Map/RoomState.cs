using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RoomState
{
    public enum RoomType
    {
        Chest,
        Enemies,
        Shop,
        Empty,
        Boss,
    }

    private Vector2Int _position;
    private RoomType _state;
    private bool _isCleared = false;

    private int _enemiesCount;

    private bool _isLootGenerated = false;
    private List<Item> _lootItems = new();
    private List<ShopState> _shopItems = new();
    public List<LootData> _groundLoot = new();

    public RoomType State => _state;
    public int EnemiesCount => _enemiesCount;
    public Vector2Int Position => _position;
    public bool IsCleared => _isCleared;

    public bool IsLootGenerated => _isLootGenerated;
    public List<Item> LootItems => _lootItems;
    public List<ShopState> ShopItems => _shopItems;
    public List<LootData> GroundLoot => _groundLoot;

    public RoomState(RoomType roomState, int enemiesCount, Vector2Int position)
    {
        _enemiesCount = enemiesCount;
        _position = position;

        RoomReset(roomState);
    }

    public void SaveGroundLoot(Transform parentObject)
    {
        _groundLoot.Clear();
        foreach (Transform obj in parentObject)
        {
            if (!obj.TryGetComponent(out Item item))
                continue;

            if (!item.BuyRequired)
                _groundLoot.Add(new(item.SourcePrefab, obj.position));
        }
    }

    public void LootAdd(Item item)
    {
        _isLootGenerated = true;
        _lootItems.Add(item);
    }

    public void RoomReset(RoomType newType)
    {
        _state = newType;
        _isCleared = false;
        _groundLoot.Clear();
        _lootItems.Clear();
        _isLootGenerated = false;
        _shopItems.Clear();

        switch (_state)
        {
            case RoomType.Chest:
                _isLootGenerated = false;
                _lootItems.Clear();
                break;
            case RoomType.Enemies:
                if (_enemiesCount <= 0) _enemiesCount = 5;
                _enemiesCount = Random.Range(Mathf.Max(1, _enemiesCount - 5), Mathf.Min(_enemiesCount + 5, 10));
                break;
            case RoomType.Shop:
                break;
            case RoomType.Empty:
                break;
            case RoomType.Boss:
                break;
        }
    }

    public void RoomClear()
    {
        _isCleared = true;
        switch (_state)
        {
            case RoomType.Enemies:
                if (Random.Range(0, 10) < 4)
                    _state = RoomType.Chest;
                else
                    _state = RoomType.Empty;
                break;
            //case RoomType.Boss:
            //    _state = RoomType.Chest;
            //    break;
            case RoomType.Chest:
                _state = RoomType.Empty;
                break;
            default:
                break;
        }
    }
}
