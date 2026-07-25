using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour
{
    [Header("Shop")]
    [SerializeField] protected bool _buyRequired = false;
    [SerializeField] protected int _price = 5;

    [Header("UI")]
    [SerializeField] private GameObject _priceUI;
    [SerializeField] private TMP_Text _priceText;


    private float _spawnTime;
    public Item SourcePrefab { get; set; }
    public bool BuyRequired => _buyRequired;

    public event Action OnBuy;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawnTime = Time.time;
    }

    private void Start()
    {
        UIUpdate();
    }

    public void SetAsShopItem(int? price = null)
    {
        if (price.HasValue)
            _price = price.Value;
        _buyRequired = true;
    }

    private void UIUpdate()
    {
        if (_buyRequired)
        {
            _priceUI.SetActive(true);
            _priceText.text = _price.ToString();
        }
        else
            _priceUI.SetActive(false);
    }

    protected abstract void OnInteract(Player player);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _spawnTime < 0.5f)
            return;

        if (collision.TryGetComponent(out Player player))
        {
            if (!_buyRequired)
                OnInteract(player);
            else
                BuyItem(player);
        }
    }

    protected void BuyItem(Player player)
    {
        if (player.Coins >= _price)
        {
            player.Coins -= _price;
            _buyRequired = false;

            UIUpdate();
            OnBuy?.Invoke();
        }
    }

    public void Throw(Vector2 direction, float force)
    {
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
