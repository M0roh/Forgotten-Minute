using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    // Monsters
    Wolf,
    ShadowGuardian,

    // Bosses
    StoneGiant,
}
[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D), typeof(NavMeshAgent))]
public abstract class EnemyAI : MonoBehaviour
{
    protected static readonly WaitForSeconds waitForSeconds2 = new(2);
    protected static readonly WaitForSeconds waitForSeconds0_005 = new(0.005f);

    [Header("Stats")]
    [SerializeField] private int _maxHealth = 10;
    private int _currentHealth;
    [SerializeField] private int _damage = 2;

    [Header("Roaming")]
    [SerializeField] private float _roamingDistanceMax = 50f;
    [SerializeField] private float _roamingDistanceMin = 10f;
    private readonly float _roamingTimerMax = 8f;
    
    [Header("Distances")]
    [SerializeField] private float _distanceStartFollow = 10f;
    [SerializeField] private float _distanceAttack = 1f;

    [Header("Others")]
    private readonly State _startingState = State.Roaming;
    [SerializeField] private EnemyType _type;
    private bool _isInvisible = false;

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTime = 0;
    private Vector3 _roamingPosition;
    private Vector3 _startingPosition;
    private bool _attackCooldown = false;

    //private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private PolygonCollider2D _polygonCollider;

    protected const string IS_RUNNING = "IsRunning";
    protected const string IS_DAMAGED = "IsDamaged";
    protected const string IS_DEATH = "Death";
    protected const string IS_ATTACK = "Attack";

    //[SerializeField] private AudioSource _soundsSource;
    //[SerializeField] private AudioClip _stepSound;
    //[SerializeField] private AudioClip _attackSound;
    //[SerializeField] private AudioClip _hitSound;

    //private float _baseStepDelay = 1f;
    //private float lastStepTime = 0f;

    public enum State { 
        Roaming,
        Follow,
        Attack,
        Death,
    };

    public int Health
    {
        get => _currentHealth;
        set
        {
            if (value > 0 && value <= MaxHealth)
                _currentHealth = value;
        }
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            Health = _maxHealth;
        }
    }
    public int Damage { get => _damage; set => _damage = value; }



    public bool Invisible
    {
        get => _isInvisible;
        set
        {
            _isInvisible = value;

            if (_isInvisible == true)
            {
                Color _color = _spriteRenderer.material.color;
                _color.a = 0.2f;
                _spriteRenderer.material.color = _color;
            }
            else
            {
                Color _color = _spriteRenderer.material.color;
                _color.a = 1f;
                _spriteRenderer.material.color = _color;
            }
        }
    }

    protected float AttackDistance { get => _distanceAttack; set => _distanceAttack = value; }
    public State CurrentState { get => _currentState; set => _currentState = value; }
    public EnemyType Type => _type;

    protected float RoamingTimerMax { get => _roamingTimerMax; }
    protected float RoamingTime { get => _roamingTime; set => _roamingTime = value; }
    protected float RoamingDistanceMax { get => _roamingDistanceMax; set => _roamingDistanceMax = value; }
    protected float RoamingDistanceMin { get => _roamingDistanceMin; set => _roamingDistanceMin = value; }

    protected float DistanceStartFollow { get => _distanceStartFollow; set => _distanceStartFollow = value; }

    protected bool AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
    //protected float BaseStepDelay { set => _baseStepDelay = value; }
    //protected AudioSource SoundsSource => _soundsSource;
    //protected AudioClip AttackSound { set => _attackSound = value; }
    //protected AudioClip StepSound { set => _stepSound = value; }

    //public Animator EnemyAnimator => _animator;
    public NavMeshAgent Agent => _navMeshAgent;
    public SpriteRenderer EnemySprite => _spriteRenderer;
    protected PolygonCollider2D AttackCollider => _polygonCollider;

    public event Action<EnemyAI> OnDeath;

    protected virtual void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        //_soundsSource = GetComponentInParent<AudioSource>();

        //_animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _polygonCollider = GetComponent<PolygonCollider2D>();

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _currentState = _startingState;
        _currentHealth = _maxHealth;
    }

    protected virtual void Start()
    {
        _polygonCollider.enabled = false;
    }

    protected virtual void Update()
    {
        if (_currentState == State.Death || DeathCheck())
            return;

        if (CheckAttackDistance())
            _currentState = State.Attack;
        else if (CheckDistance() && _currentState != State.Follow)
            _currentState = State.Follow;

        StateUpdate();

        //float speed = _navMeshAgent.velocity.magnitude;

        //float adjustedStepDelay = _baseStepDelay / Mathf.Max(speed, 0.1f);

        //if (speed > 0.1f && Time.time - lastStepTime >= adjustedStepDelay)
        //{
        //    _soundsSource.PlayOneShot(_stepSound);
        //    lastStepTime = Time.time;
        //}
    }

    protected bool DeathCheck()
    {
        if (Health <= 0 && _currentState != State.Death)
        {
            DisableAllColliders(gameObject);
            _currentState = State.Death;
            Agent.isStopped = true;
            OnDeath?.Invoke(this);
        //  _animator.SetTrigger(IS_DEATH);

            return true;
        }

        return false;
    }

    public void AttackColliderOn()
    {
        //_soundsSource.PlayOneShot(_attackSound);
        _polygonCollider.enabled = true;
    }

    //public void DamageAnim() => _animator.SetTrigger(IS_DAMAGED);

    protected bool CheckDistance()
    {
        return Vector2.Distance(Player.Instance.transform.position, transform.position) <= _distanceStartFollow;
    }

    protected bool CheckAttackDistance()
    {
        return Vector2.Distance(
            Player.Instance.transform.position,
            transform.position
        ) <= _distanceAttack;
    }

    protected virtual void StateUpdate()
    {
        switch (_currentState)
        {
            default:
            case State.Roaming:
                RoamingStateUpdate();
                break;
            case State.Attack:
                AttackStateUpdate();
                break;
            case State.Follow:
                FollowStateUpdate();
                break;
            case State.Death:
                break;
        }
    }

    protected virtual void RoamingStateUpdate()
    {
        _roamingTime -= Time.deltaTime;
        //_animator.SetBool(IS_RUNNING, true);
        if (_roamingTime <= 0 || _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.ResetPath();
            StartCoroutine(Roaming());
            _roamingTime = _roamingTimerMax;
        }
    }

    protected virtual void AttackStateUpdate()
    {
        if (!_attackCooldown)
        {
            _attackCooldown = true;
            //_animator.SetTrigger(IS_ATTACK);
            AttackColliderOn();

            ChangingFacingRotation(transform.position, Player.Instance.transform.position);
            _navMeshAgent.isStopped = true;
            AfterAttackTrigger();
        }
    }

    protected virtual void FollowStateUpdate()
    {
        //_animator.SetBool(IS_RUNNING, true);
        if (!CheckDistance())
        {
            _navMeshAgent.ResetPath();
            _currentState = State.Roaming;
            _roamingTime = _roamingTimerMax;
            return;
        }

        ChangingFacingRotation(transform.position, Player.Instance.transform.position);
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public void DisableAllColliders(GameObject enemy)
    {
        Collider[] colliders = enemy.GetComponents<Collider>();
        Collider[] childColliders = enemy.GetComponentsInChildren<Collider>(true);
        
        if (enemy.TryGetComponent<Rigidbody2D>(out var rigidBody2D))
            rigidBody2D.simulated = false;

        foreach (Collider col in colliders)
            col.enabled = false;

        foreach (Collider col in childColliders)
            col.enabled = false;
    }

    public void EnableAllColliders(GameObject enemy)
    {
        Collider[] colliders = enemy.GetComponents<Collider>();
        Collider[] childColliders = enemy.GetComponentsInChildren<Collider>(true);
        
        if (enemy.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
            rigidbody2D.simulated = true;

        foreach (Collider col in colliders)
            col.enabled = true;

        foreach (Collider col in childColliders)
            col.enabled = true;
    }

    protected void AfterDeathTrigger() => StartCoroutine(FadeEnemy());
    
    protected void AfterAttackTrigger()
    {
        _navMeshAgent.isStopped = false;
        _attackCooldown = false;
        _currentState = State.Follow;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player damageableObject = null;
        if (collision.TryGetComponent(out Player damageable))
            damageableObject = damageable;
        else if (collision.GetComponentInChildren<Player>() != null)
            damageableObject = collision.GetComponentInChildren<Player>();

        if (damageableObject == null)
            return;

        damageableObject.TakeDamage(Damage);
        _polygonCollider.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0)
            return;
        //_soundsSource.PlayOneShot(_hitSound);

        if (_currentHealth - damage <= 0)
            _currentHealth = 0;
        else
            _currentHealth -= damage;

        //DamageAnim();
    }

    protected virtual IEnumerator FadeEnemy()
    {
        yield return waitForSeconds2;

        for (float i = 1f; i >= -0.05f; i -= 0.05f)
        {
            Color color = _spriteRenderer.material.color;
            color.a = i;
            _spriteRenderer.material.color = color;

            yield return waitForSeconds0_005;
        } 

        Destroy(transform.gameObject);
    }

    protected IEnumerator Roaming() 
    {
        _startingPosition = transform.position;
        yield return StartCoroutine(Utils.GetRandomPointOnNavMesh(_startingPosition, _roamingDistanceMax, point =>
        {
            _roamingPosition = point;
        }, _navMeshAgent, _roamingDistanceMin));
        
        ChangingFacingRotation(_startingPosition, _roamingPosition);
        _navMeshAgent.SetDestination(_roamingPosition);
    }

    protected void ChangingFacingRotation(Vector3 currentPosition, Vector3 targetPosition) 
    {
        if (currentPosition.x > targetPosition.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else 
            transform.rotation = Quaternion.Euler(0, -180, 0);
    }
}
