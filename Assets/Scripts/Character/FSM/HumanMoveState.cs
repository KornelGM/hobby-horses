using UnityEngine;

public class HumanMoveState : HumanBaseState
{
    public HumanMoveState(HumanStateMachine stateMachine) : base(stateMachine) { }

    private readonly int MoveBlendTreeHash = Animator.StringToHash("Blend Tree");

    private float _velocity;
    private float _fallTimeoutDelta;
    private float TargetSpeed;
    protected float _turnSmoothVelocity;
    protected float ActualSpeed;

    public override void Enter()
    {
        StateMachine.Velocity.y = Physics.gravity.y;
        _fallTimeoutDelta = StateMachine.MovementSettings.FallTimeout;
        ActualSpeed = StateMachine.MovementSettings.MovementSpeed;
        if (StateMachine.Animator) StateMachine.Animator.CrossFadeInFixedTime(MoveBlendTreeHash, 0.1f);

        StateMachine.VirtualController.OnJumpPerformed += SwitchToJumpState;
    }

    public override void CustomUpdate()
    {
        Vector3 direction = StateMachine.VirtualController.Movement;
        StateMachine.Velocity = direction;

        if (IsGrounded())
        {
            _fallTimeoutDelta = StateMachine.MovementSettings.FallTimeout;
            StateMachine.Velocity.y = -2f;
        }
        else
        {
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                SwitchToFallState();
            }
        }

        CalculateMovementDirection(direction);
        Move();
    }

    protected virtual void SwitchToFallState()
    {
        StateMachine.SwitchState(new HumanFallState(StateMachine));
    }

    public override void Exit()
    {
        StateMachine.VirtualController.OnJumpPerformed -= SwitchToJumpState;
    }

    protected virtual void CalculateMovementDirection(Vector3 direction)
    {
        CalculateSpeed();
        SetAnimatorParameters(new(direction.x, direction.z), ActualSpeed);
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = CalculateTargetAngle(direction);
            float angle = Mathf.SmoothDampAngle(StateMachine.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, StateMachine.MovementSettings.TurnSmoothTime);
            StateMachine.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            StateMachine.Velocity.x = moveDir.x * ActualSpeed;
            StateMachine.Velocity.z = moveDir.z * ActualSpeed;
        }
    }


    protected virtual float CalculateTargetAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }


    protected void CalculateSpeed()
    {
        if (StateMachine.VirtualController.IsChargeJump) TargetSpeed = StateMachine.MovementSettings.MovementSpeed * StateMachine.MovementSettings.SprintSpeedModification;
        else TargetSpeed = StateMachine.MovementSettings.MovementSpeed;

        ActualSpeed = Mathf.SmoothDamp(ActualSpeed, TargetSpeed, ref _velocity, 0.3f);
    }

    protected virtual void SwitchToJumpState()
    {
        StateMachine.SwitchState(new HumanJumpState(StateMachine));
    }

    public override void CustomFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void CustomLateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
