using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class Sword : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int _damage = 2;
    private bool _canAttack = true;
    private readonly HashSet<EnemyAI> _hitEnemies = new();

    private Collider2D _attackCollider;
    private Animator _animator;

    public int Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            OnDamageChange?.Invoke(value);
        }
    }
    public event Action<int> OnDamageChange;

    private void Awake()
    {
        _attackCollider = GetComponent<Collider2D>();
        _attackCollider.isTrigger = true;

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        AttackColliderOff();
    }

    private void OnEnable()
    {
        GameInput.Instance.Actions.Player.Attack.performed += Attack_performed;
    }

    private void OnDisable()
    {
        GameInput.Instance.Actions.Player.Attack.performed -= Attack_performed;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canAttack)
        {
            _canAttack = false;
            _animator.SetTrigger("ATTACK");
        }
    }

    public void AttackColliderOn()
    {
        _hitEnemies.Clear();
        _attackCollider.enabled = true;
    }
    public void AttackColliderOff() => _attackCollider.enabled = false;

    public void AttackEndTrigger() => _canAttack = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_attackCollider.enabled)
            return;

        if (!collision.TryGetComponent(out EnemyAI enemy))
            return;

        if (!_hitEnemies.Add(enemy))
            return;

        enemy.TakeDamage(_damage);
    }
}
