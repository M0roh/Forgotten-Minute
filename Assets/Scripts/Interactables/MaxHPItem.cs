using UnityEngine;

public class MaxHPItem : Item
{
    [Header("Item settings")]
    [SerializeField] private int _maxHpChangeAmont = 2;
    [SerializeField] private bool _isHealPlayer = true;

    protected override void OnInteract(Player player)
    {
        player.MaxHealth += _maxHpChangeAmont;

        if (_isHealPlayer)
            player.Health += _maxHpChangeAmont;

        Destroy(gameObject);
    }
}
