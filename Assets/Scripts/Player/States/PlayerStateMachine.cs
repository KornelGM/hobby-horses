using UnityEngine;
using Cinemachine;

public class PlayerStateMachine : HumanStateMachine, IServiceLocatorComponent, IStartable
{
    [field: SerializeField] public Transform FirstPersonCamera { get; private set; }
    [field: SerializeField] public UniversalItemDatas UniversalItemDatas { get; private set; }

    public PlayerInputReader PlayerController { get; private set; }
    public InteractableSelector ItemSelector { get; private set; }
    public CharacterHand PlayerHand { get; private set; }

    public ItemServiceLocator EmptyHand;
    [HideInInspector] public IAction PreviousInteraction;

    public override void CustomStart()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out InteractableSelector itemSelector);
        ItemSelector = itemSelector;

        MyServiceLocator.TryGetServiceLocatorComponent(out CharacterHand playerHand);
        PlayerHand = playerHand;

        MyServiceLocator.TryGetServiceLocatorComponent(out PlayerInputReader playerController);
        PlayerController = playerController;

        FirstPersonCamera.IsNotNull(this, nameof(FirstPersonCamera));

        base.CustomStart();
    }

    protected override void SwitchToMoveState()
    {
        SwitchState(new PlayerMoveState(this));
    }
}