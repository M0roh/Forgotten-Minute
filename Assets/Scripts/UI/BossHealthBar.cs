using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance { get; private set; }

    [SerializeField] private Image _fill;
    [SerializeField] private GameObject _barObject;

    private readonly List<Boss> _bosses = new();

    private void Awake()
    {
        Instance = this;
        _barObject.SetActive(false);
    }

    public void RegisterBoss(Boss boss)
    {
        if (!_bosses.Contains(boss))
            _bosses.Add(boss);

        _barObject.SetActive(true);
        UpdateBar();
    }

    public void UnregisterBoss(Boss boss)
    {
        _bosses.Remove(boss);

        UpdateBar();

        if (_bosses.Count == 0)
            _barObject.SetActive(false);
    }

    public void UpdateBar()
    {
        if (_bosses.Count == 0)
            return;

        float currentHp = 0;
        float maxHp = 0;

        foreach (var boss in _bosses)
        {
            currentHp += boss.Health;
            maxHp += boss.MaxHealth;
        }

        _fill.fillAmount = currentHp / maxHp;
    }
}