using UnityEngine;

public class RoomState
{
    public enum RoomType
    {
        Loot,
        Enemies,
        Shop,
        Empty,
        Boss,
    }

    private Vector2Int _position;
    private RoomType _state;
    private int _enemiesCount;
    private bool _isCleared = false;

    public RoomType State => _state;
    public int EnemiesCount => _enemiesCount;
    public Vector2Int Position => _position;
    public bool IsCleared => _isCleared;

    public RoomState(RoomType roomState, int enemiesCount, Vector2Int position)
    {
        _enemiesCount = enemiesCount;
        _position = position;

        RoomReset(roomState);
    }

    public void RoomReset(RoomType newType)
    {
        _state = newType;
        _isCleared = false;

        switch (_state)
        {
            case RoomType.Loot:
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
                    _state = RoomType.Loot;
                else
                    _state = RoomType.Empty;
                break;
            //case RoomType.Boss:
            //    _state = RoomType.Loot;
            //    break;
            case RoomType.Loot:
                _state = RoomType.Empty;
                break;
        }
    }
}
