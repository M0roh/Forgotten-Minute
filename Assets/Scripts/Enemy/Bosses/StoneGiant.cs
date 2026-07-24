using UnityEngine;

public class StoneGiant : Boss
{
    protected override void Awake()
    {
        //BaseStepDelay = 2f;
        base.Awake();
        //AttackSound = Resources.Load<AudioClip>("Sounds/Bosses/Stone Giant/Attack");
        //StepSound = Resources.Load<AudioClip>("Sounds/Bosses/Stone Giant/Step");
    }
}