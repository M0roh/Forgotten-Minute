using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody2D), typeof(Camera), typeof(PolygonCollider2D))]
public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance => _instance;

    [Header("Health")]
    private int _health = 10;
    [SerializeField] private int _maxHealth = 10;

    [Header("Damage")]
    [SerializeField] private int _damage = 2;
    private bool _canAttack = true;

    [Header("Speed")]
    private float _currentSpeed = 0;
    [SerializeField] private int _walkSpeed = 5;
    [SerializeField] private float _sprintMultiplayer = 1.5f;

    private Vector2Int _currentRoomCoords;

    public Vector2Int CurrentRoomCoords => _currentRoomCoords;

    private Rigidbody2D _rb;
    private PolygonCollider2D _attackCollider;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        _instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _attackCollider = GetComponent<PolygonCollider2D>();

        _health = _maxHealth;
        _currentSpeed = _walkSpeed;
    }

    private void Start()
    {
        AttackColliderOff();
    }

    void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        GameInput.Instance.Actions.Player.Attack.performed += Attack_performed;

        GameInput.Instance.Actions.Player.Sprint.started += Sprint_started;
        GameInput.Instance.Actions.Player.Sprint.canceled += Sprint_canceled;
    }

    private void OnDisable()
    {
        GameInput.Instance.Actions.Player.Attack.performed -= Attack_performed;

        GameInput.Instance.Actions.Player.Sprint.started -= Sprint_started;
        GameInput.Instance.Actions.Player.Sprint.canceled -= Sprint_canceled;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Attack");
        if (_canAttack)
        {
            _canAttack = false;
            //_animator.SetTrigger(IS_ATTACK);
            
            AttackColliderOn();
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        AttackColliderOff();
        yield return new WaitForSeconds(1.5f);
        AttackEndTrigger();
    }

    public void AttackColliderOn() => _attackCollider.enabled = true;
    public void AttackColliderOff() => _attackCollider.enabled = false;

    public void AttackEndTrigger() => _canAttack = true;

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

        _rb.linearVelocity = moveVector * _currentSpeed;
    }

    public void EnterRoom(Vector2Int roomCoords) => _currentRoomCoords = roomCoords;

    public void TakeDamage(int damage) => _health -= damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_attackCollider.IsTouching(collision))
            return;

            Debug.Log("Attack detected something");
        if (collision.TryGetComponent(out EnemyAI enemy))
        {
            enemy.TakeDamage(_damage);
            Debug.Log("Attack performed");
        }
    }
}
