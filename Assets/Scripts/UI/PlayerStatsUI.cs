using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private GameObject _heartsRoot;
    [SerializeField] private GameObject _heartPrefab;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _halfHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;

    [Header("Money")]
    [SerializeField] private TMP_Text _moneyCounter;

    [Header("Damage")]
    [SerializeField] private TMP_Text _damageCounter;

    private List<Image> _spawnedHearts = new();

    private void Start()
    {
        Player_OnCoinsChange(Player.Instance.Coins);
        Player_OnHealthChange(Player.Instance.Health);
        Player_OnDamageChange(Player.Instance.Damage);
    }

    private void OnEnable()
    {
        Player.Instance.OnHealthChange += Player_OnHealthChange;
        Player.Instance.OnCoinsChange += Player_OnCoinsChange;
        Player.Instance.OnDamageChange += Player_OnDamageChange;
    }

    private void OnDisable()
    {
        Player.Instance.OnHealthChange -= Player_OnHealthChange;
        Player.Instance.OnCoinsChange -= Player_OnCoinsChange;
        Player.Instance.OnDamageChange -= Player_OnDamageChange;
    }

    private void Player_OnDamageChange(int damage)
    {
        _damageCounter.text = damage.ToString();
    }

    private void Player_OnCoinsChange(int coins)
    {
        _moneyCounter.text = coins.ToString();
    }

    private void Player_OnHealthChange(int currentHealth)
    {
        int maxHealth = Player.Instance.MaxHealth;
        int totalHeartContainers = Mathf.CeilToInt(maxHealth / 2f);

        while (_spawnedHearts.Count < totalHeartContainers)
        {
            GameObject newHeart = Instantiate(_heartPrefab, _heartsRoot.transform);
            Image heartImage = newHeart.GetComponent<Image>();
            _spawnedHearts.Add(heartImage);
        }

        for (int i = 0; i < _spawnedHearts.Count; i++)
        {
            if (i >= totalHeartContainers)
            {
                _spawnedHearts[i].gameObject.SetActive(false);
                continue;
            }

            _spawnedHearts[i].gameObject.SetActive(true);

            int heartValue = (i + 1) * 2;

            if (currentHealth >= heartValue)
                _spawnedHearts[i].sprite = _fullHeartSprite;
            else if (currentHealth == heartValue - 1)
                _spawnedHearts[i].sprite = _halfHeartSprite;
            else
                _spawnedHearts[i].sprite = _emptyHeartSprite;
        }
    }
}
