using UnityEngine;

public class DamageItem : Item
{
    [Header("Item settings")]
    [SerializeField] private int _dmgUpCount = 2;

    protected override void OnInteract(Player player)
    {
        player.Damage += _dmgUpCount;

        Destroy(gameObject);
    }
}
