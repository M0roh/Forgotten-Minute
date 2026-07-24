using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;

public class RoomRenderer : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _surface;

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

    protected Vector2Int PlayerRoom => Player.Instance.CurrentRoomCoords;

    private void Start()
    {
        StartCoroutine(RenderRoom(PlayerRoom));
    }

    private IEnumerator RenderRoom(Vector2Int roomPosition)
    {
        yield return null;
        if (!MapGenerator.Instance.RoomExists(roomPosition))
            yield break;

        var room = MapGenerator.Instance.GetRoom(roomPosition);

        _leftDoor.SetActive(false);
        _rightDoor.SetActive(false);
        _upDoor.SetActive(false);
        _downDoor.SetActive(false);

        if (MapGenerator.Instance.RoomExists(roomPosition + Vector2Int.left))
            _leftDoor.SetActive(true);
        if (MapGenerator.Instance.RoomExists(roomPosition + Vector2Int.right))
            _rightDoor.SetActive(true);
        if (MapGenerator.Instance.RoomExists(roomPosition + Vector2Int.up))
            _upDoor.SetActive(true);
        if (MapGenerator.Instance.RoomExists(roomPosition + Vector2Int.down))
            _downDoor.SetActive(true);
        yield return null;

        switch (room.State)
        {
            case RoomState.RoomType.Loot:
                break;
            case RoomState.RoomType.Enemies:
                break;
            case RoomState.RoomType.Shop:
                break;
            case RoomState.RoomType.Empty:
                break;
            case RoomState.RoomType.Boss:
                break;
        }
        yield return null;

        _surface.BuildNavMesh();
    }

    private void MoveToRoom(Vector2Int direction)
    {
        var next = PlayerRoom + direction;
        if (!MapGenerator.Instance.IsRoomPossible(next) || !MapGenerator.Instance.IsRoom(next))
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

        Player.Instance.EnterRoom(next);
        StartCoroutine(RenderRoom(next));
    }

    public void MoveLeftRoom() => MoveToRoom(Vector2Int.left);
    public void MoveRightRoom() => MoveToRoom(Vector2Int.right);
    public void MoveUpRoom() => MoveToRoom(Vector2Int.up);
    public void MoveDownRoom() => MoveToRoom(Vector2Int.down);
}
