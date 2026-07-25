using UnityEngine;

public class HealItem : Item
{
    [Header("Item settings")]
    [SerializeField, Min(0)] private int _healAmount = 2;

    protected override void OnInteract(Player player)
    {
        if (player.Health == player.MaxHealth) return;

        if (player.Health + _healAmount > player.MaxHealth)
            player.Health = player.MaxHealth;
        else
            player.Health += _healAmount;

        Destroy(gameObject);
    }
}
