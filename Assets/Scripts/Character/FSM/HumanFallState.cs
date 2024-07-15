using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanFallState : HumanBaseState
{
    private readonly int FallHash = Animator.StringToHash("Fall");

    public HumanFallState(HumanStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StateMachine.Velocity.y = 0f;

        if(StateMachine.Animator) StateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
    }

    public override void CustomUpdate()
    {
        ApplyGravity();
        Move();

        if (IsGrounded())
        {
            SwitchToMoveState();
        }
    }

    protected virtual void SwitchToMoveState()
    {
        StateMachine.SwitchState(new HumanMoveState(StateMachine));
    }
    
    public override void Exit()
    {
       // StateMachine.Footsteps.PlayEvent(HumanAudio.FOOTSTEPS);
    }

    public override void CustomFixedUpdate()
    {
    }
}