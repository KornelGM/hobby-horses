using UnityEngine;

public class HobbyHorseMovementState : State
{
    private PlayerCameraRotator _cameraRotator;
    private HobbyHorseStateMachine _stateMachine;
    private HobbyHorseMovement _movement;
    private GravityCharacterController _gravityController;
    private IVirtualController _virtualController;
    private SlowMotionManager _slowMotionManager;
    private InputManager _inputManager;

    public HobbyHorseMovementState(HobbyHorseStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _cameraRotator);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _movement);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _stateMachine);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _virtualController);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _gravityController);


        SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _slowMotionManager);
        SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _inputManager);
    }

    public override void Enter()
    {
        _inputManager.SwitchInputMap(true, _inputManager.GameplayCategoryName);
        _virtualController.OnFirstInteractionPerformed += () => _cameraRotator.SwitchFreeCamera(true);
        _virtualController.OnFirstInteractionCancelled += () => _cameraRotator.BackToCenterPosition();

        _virtualController.OnJumpCancelled += Jump;
        _virtualController.OnJumpCancelled += CancelJumpCharge;
        _virtualController.OnSlowMotionPerformed += ChangeTime;
    }

    public override void Exit()
    {
        _inputManager.SwitchInputMap(false, _inputManager.GameplayCategoryName);
        _virtualController.OnFirstInteractionPerformed -= () => _cameraRotator.SwitchFreeCamera(true);
        _virtualController.OnFirstInteractionCancelled -= () => _cameraRotator.BackToCenterPosition();

        _virtualController.OnJumpCancelled -= Jump;
        _virtualController.OnJumpCancelled -= CancelJumpCharge;
        _virtualController.OnSlowMotionPerformed -= ChangeTime;
    }

    public override void CustomUpdate()
    {
        _movement.Move(_virtualController.Movement, IsGrounded());

        if (_cameraRotator.FreeCamera)
            _cameraRotator.Rotate(_virtualController.Mouse.y, _virtualController.Mouse.x);
        else
            _cameraRotator.CameraFollowTarget();

        JumpCharging();
        _gravityController.ApplyGravity(IsGrounded());
    }

    public override void CustomFixedUpdate()
    {

    }

    private void ChangeTime()
    {
        _slowMotionManager.ChangeTime();
    }

    private void JumpCharging()
    {
        if (!AbleToJump() && !_gravityController.AbleToChargeForceOnAir) return;

       _gravityController.ChargingForce(_virtualController.IsChargeJump);
    }

    private void CancelJumpCharge()
    {
        _gravityController.CancelCharge();
    }

    private void Jump()
    {
        if (!AbleToJump()) return;
        _gravityController.Jump();
    }

    protected bool IsGrounded()
    {
        RaycastHit? hit = GroundRaycast(_stateMachine.MovementSettings.GroundedOffset);
        return hit != null;
    }

    protected bool AbleToJump()
    {
        RaycastHit? hit = GroundRaycast(_stateMachine.MovementSettings.GroundedOffset + _stateMachine.MovementSettings.JumpAdditionalOffset);
        return hit != null;
    }

    protected RaycastHit? GroundRaycast(float dist)
    {
        if (!Physics.Raycast(_stateMachine.transform.position, Vector3.down, out RaycastHit FallRayCast, dist, _stateMachine.MovementSettings.GroundLayers, QueryTriggerInteraction.Ignore)) return null;
        return FallRayCast;
    }
}
