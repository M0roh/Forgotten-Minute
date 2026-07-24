public abstract class Boss : EnemyAI
{

    protected override void Start()
    {
        base.Start();
        CurrentState = State.Follow;
    }

    protected override void Update()
    {
        if (DeathCheck())
            return;

        if (CheckAttackDistance())
            CurrentState = State.Attack;
        else
            CurrentState = State.Follow;

        StateUpdate();
    }

    protected override void StateUpdate()
    {
        switch (CurrentState)
        {
            case State.Roaming:
                CurrentState = State.Follow;
                break;
            case State.Attack:
                AttackStateUpdate();
                break;
            default:
            case State.Follow:
                FollowStateUpdate();
                break;
            case State.Death:
                break;
        }
    }
    public void AttackColliderON() => AttackCollider.enabled = true;
    public virtual void AttackColliderOFF() => AttackCollider.enabled = false;
}