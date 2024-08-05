public class PlayerMoveState : HumanBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) 
    {
        ServiceLocator serviceLocator = stateMachine.MyServiceLocator;
        serviceLocator.TryGetServiceLocatorComponent(out _itemSelector);
        serviceLocator.TryGetServiceLocatorComponent(out _cameraRotator);
        serviceLocator.TryGetServiceLocatorComponent(out _playerRotator);
        serviceLocator.TryGetServiceLocatorComponent(out _move);
        serviceLocator.TryGetServiceLocatorComponent(out _virtualController);
        serviceLocator.TryGetServiceLocatorComponent(out _hologramController);
        serviceLocator.TryGetServiceLocatorComponent(out _gravityController);

        _itemSelector.IsNotNull(this, nameof(_itemSelector));
    }

    protected new PlayerStateMachine StateMachine { get => (PlayerStateMachine) base.GetStateMachine();  }

    private PlayerCameraRotator _cameraRotator;
    private InteractableSelector _itemSelector;
    private ICharacterRotator _playerRotator;
    private IMove _move;
    private IVirtualController _virtualController;
    private CharacterInteraction _characterInteraction;
    private HologramController _hologramController;
    private GravityCharacterController _gravityController;

    public override void Enter()
    {
        _characterInteraction = new CharacterInteraction(StateMachine.MyServiceLocator);
        _virtualController.OnJumpPerformed += Jump;
        _virtualController.OnFirstInteractionPerformed += PrimaryInteraction;
        _virtualController.OnSecondInteractionPerformed += SecondaryInteraction;
        _virtualController.OnAdditiveInteractionPerformed += AdditiveInteraction;
        _virtualController.OnSlowMotionPerformed += MoreIfnoInteraction;
        _virtualController.OnFirstInteractionCancelled += CancelPrimaryInteraction;
        _virtualController.OnSecondInteractionCancelled += CancelSecondaryInteraction;
        _virtualController.OnAdditiveInteractionCancelled += CancelAdditiveInteraction;
        _virtualController.OnSlowMotionCancelled += CancelMoreInfo;

        _itemSelector.DetectItems();
    }

    public override void Exit()
    {
        _virtualController.OnJumpPerformed -= Jump;
        _virtualController.OnFirstInteractionPerformed -= PrimaryInteraction;
        _virtualController.OnSecondInteractionPerformed -= SecondaryInteraction;
        _virtualController.OnAdditiveInteractionPerformed -= AdditiveInteraction;
        _virtualController.OnSlowMotionPerformed -= MoreIfnoInteraction;
        _virtualController.OnFirstInteractionCancelled -= CancelPrimaryInteraction;
        _virtualController.OnSecondInteractionCancelled -= CancelSecondaryInteraction;
        _virtualController.OnAdditiveInteractionCancelled -= CancelAdditiveInteraction;
        _virtualController.OnSlowMotionCancelled -= CancelMoreInfo;
        _characterInteraction = null;
        _itemSelector.Deselect(true);
        _hologramController.StopDisplayingHologram();
        _gravityController.SetGravity(0);
    }

    public override void CustomUpdate()
    {
        _move.Move(_virtualController.Movement, _virtualController.IsChargeJump);
        _playerRotator.Rotate(_virtualController.Mouse.x);
        //_cameraRotator.Rotate(_virtualController.Mouse.y);
        _gravityController.ApplyGravity(IsGrounded());

        if (_hologramController.Projecting)
        {
            _hologramController.UpdateHologram();
        }
        else
        {
            _itemSelector.DetectItems();
        }
    }

    public override void CustomFixedUpdate()
    {

    }
   
    private void Jump()
    {
        if (!AbleToJump()) return;
        _gravityController.Jump();
    }

    private void PrimaryInteraction() => Interact(InteractionType.PrimaryInteraction);
    private void SecondaryInteraction() => Interact(InteractionType.SecondaryInteraction);
    private void AdditiveInteraction() => Interact(InteractionType.AdditiveInteraction);
    private void MoreIfnoInteraction() => Interact(InteractionType.MoreInfo);

    private void CancelPrimaryInteraction() => Interact(InteractionType.CancelPrimaryInteraction);
    private void CancelSecondaryInteraction() => Interact(InteractionType.CancelSecondaryInteraction);
    private void CancelAdditiveInteraction() => Interact(InteractionType.CancelAdditiveInteraction);
    private void CancelMoreInfo() => Interact(InteractionType.CancelMoreInfo);

    private void Interact(InteractionType interactionType)
    {
        _characterInteraction.TryInteract(interactionType, _itemSelector.ItemToInteract);
    }

    public override void CustomLateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
