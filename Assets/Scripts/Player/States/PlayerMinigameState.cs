using UnityEngine;

public class PlayerMinigameState : HumanBaseState
{
    public PlayerMinigameState(PlayerStateMachine stateMachine, IControllable controllable) : base(stateMachine)
    {
        ServiceLocator serviceLocator = stateMachine.MyServiceLocator;
        serviceLocator.TryGetServiceLocatorComponent(out _itemSelector);
        serviceLocator.TryGetServiceLocatorComponent(out _virtualController);
        _controllable = controllable;
    }

    protected new PlayerStateMachine StateMachine { get => (PlayerStateMachine)base.GetStateMachine(); }
    private IVirtualController _virtualController;
    private IControllable _controllable;
    private InteractableSelector _itemSelector;
    private CharacterInteraction _characterInteraction;

    public override void Enter()
    {
        _characterInteraction = new CharacterInteraction(StateMachine.MyServiceLocator);
        _controllable.OnPlayerUnlocked += SwitchToMoveState;
        _controllable.OnControllingInterrupted += SwitchToMoveState;
        _controllable.OnControllingEntered(_virtualController);

        _virtualController.OnFirstInteractionCancelled += CancelPrimaryInteraction;

        _itemSelector.DetectItems();
    }

    public override void Exit()
    {
        _controllable.OnPlayerUnlocked -= SwitchToMoveState;
        _controllable.OnControllingInterrupted -= SwitchToMoveState;
        _controllable.OnControllingExited(_virtualController);

        _virtualController.OnFirstInteractionCancelled -= CancelPrimaryInteraction;

        _itemSelector.Deselect(true);
    }

    public override void CustomUpdate()
    {
        _controllable.ControlObject(_virtualController);
    }

    public override void CustomFixedUpdate()
    {

    }

    private void SwitchToMoveState()
    {
        StateMachine.SwitchState(new PlayerMoveState(StateMachine));
    }


    private void CancelPrimaryInteraction() => Interact(InteractionType.CancelPrimaryInteraction);

    private void Interact(InteractionType interactionType)
    {
        _characterInteraction.TryInteract(interactionType, _itemSelector.ItemToInteract);
    }

}
