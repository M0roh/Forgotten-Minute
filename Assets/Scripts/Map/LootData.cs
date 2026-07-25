using UnityEngine;

public class LootData
{
    private Item _item;
    private Vector3 _position;

    public Item Item => _item;
    public Vector3 Position => _position;

    public LootData(Item item, Vector3 position)
    {
        _position = position;

        _item = item;
    }
}

