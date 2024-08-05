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

    private Vector3 _inputMouse;
    private Vector3 _inputMovement;

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

        _movement.OnLanding += DoShakeCamera;
    }

    public override void Exit()
    {
        _inputManager.SwitchInputMap(false, _inputManager.GameplayCategoryName);
        _virtualController.OnFirstInteractionPerformed -= () => _cameraRotator.SwitchFreeCamera(true);
        _virtualController.OnFirstInteractionCancelled -= () => _cameraRotator.BackToCenterPosition();

        _virtualController.OnJumpCancelled -= Jump;
        _virtualController.OnJumpCancelled -= CancelJumpCharge;
        _virtualController.OnSlowMotionPerformed -= ChangeTime;

        _movement.OnLanding -= DoShakeCamera;
    }

    public override void CustomUpdate()
    {
        _inputMovement = _virtualController.Movement;
        _inputMouse = _virtualController.Mouse;

        _movement.CalculateInput(_inputMovement, IsGrounded());

        JumpCharging();
    }

    public override void CustomFixedUpdate()
    {
        _movement.Move();
        _movement.Rotate();

        if (_cameraRotator.FreeCamera)
            _cameraRotator.Rotate(_inputMouse.y, _inputMouse.x);
        else
            _cameraRotator.CameraFollowTarget();

        _gravityController.ApplyGravity(IsGrounded());
    }

    public override void CustomLateUpdate()
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

    private void DoShakeCamera()
    {
        _cameraRotator.ShakeCamera(_gravityController.CurrGravity);
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
