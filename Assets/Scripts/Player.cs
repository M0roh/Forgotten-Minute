using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Camera))]
public class Player : MonoBehaviour
{
    [Header("Health")]
    private int _health = 10;
    [SerializeField] private int _maxHealth = 10;

    [Header("Damage")]
    [SerializeField] private int _damage = 2;
    [SerializeField] private int _attackCooldown = 1;

    [Header("Speed")]
    private float _currentSpeed = 0;
    [SerializeField] private int _walkSpeed = 5;
    [SerializeField] private float _sprintMultiplayer = 1.5f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _health = _maxHealth;
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

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentSpeed = _walkSpeed;
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentSpeed = _walkSpeed * _sprintMultiplayer;
    }

    void Move()
    {
        var moveVector = GameInput.Instance.GetMoveVector();

        _rb.linearVelocity = moveVector * _currentSpeed;
    }
}
