public class CharacterInteraction
{
    public CharacterInteraction(ServiceLocator character) 
    {
        _character = character;
        character.TryGetServiceLocatorComponent(out _actionsReference);
        character.TryGetServiceLocatorComponent(out _characterHand);
    }

    private ServiceLocator _character;
    private IAvailableActions _actionsReference;
    private CharacterHand _characterHand;

    public void TryInteract(InteractionType interactionType, ServiceLocator interactionObject)
    {
        IAction action = ChooseCorrectAction(interactionType);
        if (action == null) return;

        PerformAction(action, interactionObject);
    }

    private void PerformAction(IAction action, ServiceLocator interactionObject)
    {
        action.Perform(_character, interactionObject, _characterHand.ItemInHand);
    }

    private IAction ChooseCorrectAction(InteractionType interactionType)
    {
        return _actionsReference.GetAvailableAction(interactionType);
    }
}