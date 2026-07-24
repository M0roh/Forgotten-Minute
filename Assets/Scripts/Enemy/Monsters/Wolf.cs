using UnityEngine;

public class Wolf : EnemyAI
{
    protected override void Awake()
    {
        //BaseStepDelay = 2f;
        base.Awake();
        //AttackSound = Resources.Load<AudioClip>("Sounds/Enemies/Wolf/Attack");
        //StepSound = Resources.Load<AudioClip>("Sounds/Enemies/Wolf/Step");
    }
}