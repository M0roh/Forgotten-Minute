using System.Collections;
using UnityEngine;

public class ShadowGuardian : EnemyAI
{
    [SerializeField] private float _distanceOffInvisible = 1.8f;

    protected override void Awake()
    {
        base.Awake();
        //AttackSound = Resources.Load<AudioClip>("Sounds/Enemies/Shadow Guardian/SwordAttack");
        //StepSound = Resources.Load<AudioClip>("Sounds/Enemies/Shadow Guardian/Step");
    }

    protected override void StateUpdate()
    {
        base.StateUpdate();

        if (Vector2.Distance(transform.position, Player.Instance.transform.position) > _distanceOffInvisible && CurrentState != State.Attack)
            Invisible = true;
        else
            Invisible = false;
    }

    protected override IEnumerator FadeEnemy()
    {
        yield return waitForSeconds2;

        Destroy(transform.gameObject);
    }
}