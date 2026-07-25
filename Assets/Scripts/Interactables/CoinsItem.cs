using UnityEngine;

public class CoinsItem : Item
{
    [Header("Item settings")]
    [SerializeField, Min(1)] private int _randomMin = 1;
    [SerializeField, Min(1)] private int _randomMax = 2;

    protected override void OnInteract(Player player)
    {
        player.Coins += Random.Range(_randomMin, _randomMax);

        Destroy(gameObject);
    }
}
