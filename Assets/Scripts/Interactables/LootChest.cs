using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LootChest : MonoBehaviour
{
    private List<Item> _items = new();
    private Transform _itemsParent;

    public event Action OnOpen;

    public void SetLoot(List<Item> items, Transform itemsParent)
    {
        if (_items != null && _items.Count <= 0)
            _items = items;

        _itemsParent = itemsParent;
    }

    public void OnInteract()
    {
        OnOpen?.Invoke();
        foreach (var item in _items)
        {
            var spawnedItem = Instantiate(
                item,
                transform.position,
                Quaternion.identity,
                _itemsParent
            );

            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            spawnedItem.Throw(direction, 20f);
            spawnedItem.SourcePrefab = item;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _))
            OnInteract();
    }
}
