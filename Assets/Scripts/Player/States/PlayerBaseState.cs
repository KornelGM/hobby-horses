using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    protected const float CrossFadeDuration = 0.1f;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move()
    {
        stateMachine.Controller.Move(stateMachine.Velocity * Time.deltaTime);
    }

    protected void ApplyGravity()
    {
        if (stateMachine.Velocity.y > Physics.gravity.y)
        {
            stateMachine.Velocity.y += Physics.gravity.y * stateMachine.TimeManager.GetDeltaTime();
        }
    }

    protected bool IsGrounded()
    {
        Vector3 spherePosition = new Vector3(stateMachine.transform.position.x, stateMachine.transform.position.y - stateMachine.MovementSettings.GroundedOffset, stateMachine.transform.position.z);
        return Physics.CheckSphere(spherePosition, stateMachine.MovementSettings.GroundedRadius, stateMachine.MovementSettings.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    protected void SetAnimatorParameters(float actualHorizontal, float actualVerticalSpeed)
    {
        if (stateMachine.Animator == null) return;
        stateMachine.Animator.SetFloat("ZMovment", actualVerticalSpeed, stateMachine.MovementSettings.VerticalMoveSmoother, stateMachine.TimeManager.GetDeltaTime());
        stateMachine.Animator.SetFloat("XMovment", actualHorizontal, stateMachine.MovementSettings.HorizontalMoveSmoother, stateMachine.TimeManager.GetDeltaTime());
    }
}
