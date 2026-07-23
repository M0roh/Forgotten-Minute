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

    private RoomType _state;
    private int _enemiesCount;

    public RoomType State => _state;
    public int EnemiesCount => _enemiesCount;

    public RoomState(RoomType roomState, int enemiesCount)
    {
        _state = roomState;
        _enemiesCount = enemiesCount;
    }

    public void RoomReset()
    {
        _enemiesCount = Random.Range(Mathf.Min(1, _enemiesCount - 5), Mathf.Min(_enemiesCount + 5, 10));
    }

    public void RoomClear()
    {
        if (_state == RoomType.Enemies)
        {
            if (Random.Range(0, 10) < 4)
                _state = RoomType.Loot;
            else
                _state = RoomType.Empty;
        }
    }
}
