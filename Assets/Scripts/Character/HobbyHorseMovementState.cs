using UnityEngine;

public class HobbyHorseMovementState : State
{
    private PlayerCameraRotator _cameraRotator;
    private HobbyHorseStateMachine _stateMachine;
    private ICharacterRotator _playerRotator;
    private HobbyHorseMovement _movement;
    private GravityCharacterController _gravityController;
    private IVirtualController _virtualController;

    public HobbyHorseMovementState(HobbyHorseStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _cameraRotator);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _movement);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _stateMachine);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _virtualController);
        _stateMachine.MyServiceLocator.TryGetServiceLocatorComponent(out _gravityController);
    }

    public override void Enter()
    {
        _virtualController.OnFirstInteractionPerformed += () => _cameraRotator.SwitchFreeCamera(true);
        _virtualController.OnFirstInteractionCancelled += () => _cameraRotator.BackToCenterPosition();
    }

    public override void CustomUpdate()
    {
        _movement.Move(_virtualController.Movement);

        if (_cameraRotator.FreeCamera)
            _cameraRotator.Rotate(_virtualController.Mouse.y, _virtualController.Mouse.x);
        else
            _cameraRotator.CameraFollowTarget();

        _gravityController.ApplyGravity(IsGrounded());
    }

    public override void CustomFixedUpdate()
    {
        _virtualController.OnFirstInteractionPerformed -= () => _cameraRotator.SwitchFreeCamera(true);
        _virtualController.OnFirstInteractionCancelled -= () => _cameraRotator.BackToCenterPosition();
    }

    public override void Exit()
    {
        _cameraRotator.SwitchFreeCamera(false);
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
