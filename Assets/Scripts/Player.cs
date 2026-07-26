using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(Camera))]
public class Player : MonoBehaviour 
{
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    private static Player _instance;
    public static Player Instance => _instance;

    [Header("Health")]
    private int _health = 10;
    [SerializeField] private int _maxHealth = 10;

    [Header("Speed")]
    private float _currentSpeed = 0;
    [SerializeField] private int _walkSpeed = 5;
    [SerializeField] private float _sprintMultiplayer = 1.5f;

    private int _coins = 0;

    private Vector2Int _currentRoomCoords;
    private readonly HashSet<EnemyAI> _hitEnemies = new();

    public Vector2Int CurrentRoomCoords => _currentRoomCoords;

    private Sword _sword;
    private Rigidbody2D _rb;
    private Animator _animator;

    public Sword Sword => _sword;
    public int Health
    {
        get => _health;
        set
        {
            if (_health < 0 || _health > _maxHealth)
                return;

            _health = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChange?.Invoke(value);
        }
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            if (_maxHealth <= 1)
                return;

            _maxHealth = value;
            OnHealthChange?.Invoke(_health);
        }
    }

    public int Coins
    {
        get => _coins;
        set
        {
            _coins = value;
            OnCoinsChange?.Invoke(value);
        }
    }

    public event Action<int> OnHealthChange;
    public event Action<int> OnCoinsChange;
    public event Action OnDeath;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        _instance = this;

        _rb = GetComponent<Rigidbody2D>();

        _animator = GetComponent<Animator>();

        _sword = GetComponentInChildren<Sword>();

        Health = _maxHealth;
        _currentSpeed = _walkSpeed;
    }

    void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        GameInput.Instance.Actions.Player.Sprint.started += Sprint_started;
        GameInput.Instance.Actions.Player.Sprint.canceled += Sprint_canceled;
    }

    private void OnDisable()
    { 
        GameInput.Instance.Actions.Player.Sprint.started -= Sprint_started;
        GameInput.Instance.Actions.Player.Sprint.canceled -= Sprint_canceled;
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentSpeed = _walkSpeed * _sprintMultiplayer;
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentSpeed = _walkSpeed;
    }

    private void Move() 
    {
        var moveVector = GameInput.Instance.GetMoveVector();

        if (moveVector.magnitude >= 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveVector.x),
                1,
                1
            );

            if (!_animator.GetBool(IsRunHash))
                _animator.SetBool(IsRunHash, true);
            else if (_animator.GetBool(IsRunHash))
                _animator.SetBool(IsRunHash, false);
        }

        _rb.linearVelocity = moveVector * _currentSpeed;
    }

    public void EnterRoom(Vector2Int roomCoords) => _currentRoomCoords = roomCoords;

    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;

        if (Health - damage <= 0)
        {
            Health = 0;
            _animator.SetTrigger("Death");
            OnDeath?.Invoke();
        }
        else
            Health -= damage;
    }
}
