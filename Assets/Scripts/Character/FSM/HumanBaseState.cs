using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanBaseState : State
{
    protected  HumanStateMachine StateMachine { get => GetStateMachine(); }
    private readonly HumanStateMachine _stateMachine;
    protected virtual HumanStateMachine GetStateMachine() => _stateMachine;

    protected const float CrossFadeDuration = 0.1f;

    protected HumanBaseState(HumanStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    protected void Move()
    {
        StateMachine.Controller.Move(StateMachine.Velocity * Time.deltaTime);
    }

    protected void ApplyGravity()
    {
        if (StateMachine.Velocity.y > Physics.gravity.y)
        {
            StateMachine.Velocity.y += Physics.gravity.y * StateMachine.TimeManager.GetDeltaTime();
        }
    }

    protected bool IsGrounded()
    {
        RaycastHit? hit = GroundRaycast(StateMachine.MovementSettings.GroundedOffset);
        return hit != null;
    }

    protected bool AbleToJump()
    {
        RaycastHit? hit = GroundRaycast(StateMachine.MovementSettings.GroundedOffset + StateMachine.MovementSettings.JumpAdditionalOffset);
        return hit != null;
    }

    protected RaycastHit? GroundRaycast(float dist)
    {
        if(!Physics.Raycast(StateMachine.transform.position, Vector3.down, out RaycastHit FallRayCast, dist, StateMachine.MovementSettings.GroundLayers, QueryTriggerInteraction.Ignore))return null;
        return FallRayCast;
    }

    protected void SetAnimatorParameters(Vector2 direction, float actualSpeed)
    {
        if (StateMachine.Animator == null) return;
        StateMachine.Animator.SetFloat("XMovment", Mathf.Clamp((direction.magnitude * actualSpeed) / (StateMachine.MovementSettings.MovementSpeed * StateMachine.MovementSettings.SprintSpeedModification), 0f, 1f), 0.1f, Time.deltaTime);
    }
}
