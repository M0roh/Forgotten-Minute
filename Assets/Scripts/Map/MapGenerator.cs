using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static RoomState;

public class MapGenerator : MonoBehaviour
{
    private static MapGenerator _instance;
    public static MapGenerator Instance => _instance;

    [Header("Map")]
    [SerializeField] private int _mapSize = 15;
    [SerializeField] private int _minPathLenght = 5;
    [SerializeField] private int _maxTreePathLenght = 8;

    [Header("Rooms")]
    [SerializeField] private int _enemiesMaxCount = 6;

    [Header("World Reset")]
    [SerializeField] private int _worldResetTimeMax = 60;
    private float _worldResetTimer = 60;

    private static readonly Vector2Int[] ALL_DIRECTIONS =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private RoomState[,] _map;
    private List<Vector2Int> _existingRooms = new();

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        _instance = this;

        _worldResetTimer = _worldResetTimeMax;
    }

    private async UniTaskVoid Start()
    {
        await StartGeneration();
    }

    private void Update()
    {
        _worldResetTimer -= Time.deltaTime;

        if (_worldResetTimer <= 0)
        {
            _worldResetTimer = _worldResetTimeMax;
            ResetWorld();
        }
    }

    public RoomState GetRoom(Vector2Int position)
    {
        if (!IsRoomPossible(position))
            return null;
        return _map[position.x, position.y];
    }

    public async UniTask StartGeneration()
    {
        _map = new RoomState[_mapSize,_mapSize];
        _existingRooms = new();

        var startCords = new Vector2Int(_mapSize / 2, _mapSize / 2);
        _map[startCords.x, startCords.y] = new(RoomType.Empty, 0, startCords);
        _existingRooms.Add(startCords);
        Player.Instance.EnterRoom(startCords);

        int totalRoomsCount = 1;
        int minRoomsCount = _mapSize * _mapSize / 10;

        totalRoomsCount += GenerateBossPath(startCords);
        await UniTask.Yield(); 

        do
        {
            totalRoomsCount += GenerateTreePath();
            await UniTask.Yield();
        } while (totalRoomsCount < minRoomsCount);

        DebugMap();
    }

    private void DebugMap()
    {
        StringBuilder mapLog = new();
        for (int y = _mapSize - 1; y >= 0; y--)
        {
            string line = "";

            for (int x = 0; x < _mapSize; x++)
            {
                if (_map[x, y] == null)
                {
                    line += " . ";
                    continue;
                }

                line += _map[x, y].State switch
                {
                    RoomType.Boss => " B ",
                    RoomType.Enemies => " E ",
                    RoomType.Empty => " S ",
                    RoomType.Loot => " C ",
                    RoomType.Shop => " $ ",
                    _ => " ? "
                };
            }

            mapLog.AppendLine(line);
        }
        Debug.Log(mapLog.ToString());
    }

    private int GenerateBossPath(Vector2Int startPos)
    {
        int roomsGenerated = 0;

        int bossPathLenght = Random.Range(Mathf.Min(_minPathLenght, _mapSize / 2), _mapSize - 1);
        var lastProcessedRoom = startPos;
        for (int i = 0; i < bossPathLenght; i++)
        {
            var newRoomCoords = FindNextRoom(lastProcessedRoom);
            if (newRoomCoords == null)
                break;

            _map[newRoomCoords.Value.x, newRoomCoords.Value.y] = new(GenerateRoomType(), Random.Range(1, _enemiesMaxCount), newRoomCoords.Value);

            roomsGenerated++;
            _existingRooms.Add(newRoomCoords.Value);
            lastProcessedRoom = newRoomCoords.Value;
        }
        _map[lastProcessedRoom.x, lastProcessedRoom.y] = new(RoomType.Boss, 1, lastProcessedRoom);
        _existingRooms.Remove(lastProcessedRoom);

        return roomsGenerated;
    }

    private int GenerateTreePath()  
    {
        int roomsGenerated = 0;

        int pathLenght = Random.Range(1, Mathf.Min(_maxTreePathLenght, _mapSize / 2));
        var lastProcessedRoom = _existingRooms[Random.Range(0, _existingRooms.Count - 1)];
        for (int i = 0; i < pathLenght; i++)
        {
            var newRoomCoords = FindNextRoom(lastProcessedRoom);
            if (newRoomCoords == null)
                break;

            _map[newRoomCoords.Value.x, newRoomCoords.Value.y] = new(GenerateRoomType(), Random.Range(1, _enemiesMaxCount), newRoomCoords.Value);

            roomsGenerated++;
            _existingRooms.Add(newRoomCoords.Value);
            lastProcessedRoom = newRoomCoords.Value;
        }

        return roomsGenerated;
    }

    private RoomType GenerateRoomType()
    {
        int roll = Random.Range(0, 100);

        if (roll < 60)
            return RoomType.Enemies;

        if (roll < 75)
            return RoomType.Empty;

        if (roll < 90)
            return RoomType.Shop;

        return RoomType.Loot;
    }

    private Vector2Int? FindNextRoom(Vector2Int current)
    {
        var directions = ALL_DIRECTIONS.OrderBy(x => Random.value).ToArray();

        foreach (var dir in directions)
        {
            var next = current + dir;

            if (!IsRoomPossible(next) || IsRoom(next))
                continue;

            if (CountNeighbours(next) > 1)
                continue;

            return next;
        }

        return null;
    }

    public bool IsRoom(Vector2Int roomCoords) => _map[roomCoords.x, roomCoords.y] != null;

    public bool IsRoomPossible(Vector2Int roomCoords) => 
        roomCoords.x > 0 && roomCoords.x < _mapSize &&
        roomCoords.y > 0 && roomCoords.y < _mapSize;

    public bool RoomExists(Vector2Int roomCoords) => IsRoomPossible(roomCoords) && IsRoom(roomCoords);

    public int CountNeighbours(Vector2Int pos)
    {
        int count = 0;

        foreach (var dir in ALL_DIRECTIONS)
        {
            if (IsRoomPossible(pos + dir) && IsRoom(pos + dir))
                count++;
        }

        return count;
    }

    public void ResetWorld()
    {
        foreach (var i in _map)
        {
            if (i == null)
                continue;

            else if (i.Position != Player.Instance.CurrentRoomCoords 
                && i.State != RoomType.Boss)
                i.RoomReset(GenerateRoomType());
        }
        DebugMap();
    }
}
