using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanJumpState : HumanBaseState
{
    public HumanJumpState(HumanStateMachine stateMachine) : base(stateMachine) { }

    private readonly int JumpHash = Animator.StringToHash("Jump");

    public override void Enter()
    {
        //StateMachine.Footsteps.PlayEvent(HumanAudio.FOOTSTEPS);
        StateMachine.Velocity = new Vector3(StateMachine.Velocity.x, StateMachine.MovementSettings.JumpForce, StateMachine.Velocity.z);
        if(StateMachine.Animator) StateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
    }

    public override void CustomUpdate()
    {
        ApplyGravity();
        if (StateMachine.Velocity.y <= 0f)
        {
            SwitchToFallState();
        }

        Move();
    }

    protected virtual void SwitchToFallState()
    {
        StateMachine.SwitchState(new HumanFallState(StateMachine));
    }

    public override void Exit()
    {
    }

    public override void CustomFixedUpdate()
    {
    }
}