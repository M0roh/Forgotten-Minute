using UnityEngine;

public class HealItem : Item
{
    [Header("Item settings")]
    [SerializeField, Min(0)] private int _healAmont = 2;

    protected override void OnInteract(Player player)
    {
        if (player.Health == player.MaxHealth) return;

        if (player.Health + _healAmont > player.MaxHealth)
            player.Health = player.MaxHealth;
        else
            player.Health += _healAmont;

        Destroy(gameObject);
    }
}
