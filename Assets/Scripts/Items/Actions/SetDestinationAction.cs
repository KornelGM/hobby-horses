using Pathfinding;

public class SetDestinationAction : BaseAction
{
    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    {
        InteractableDetector _interactableDetector;

        if(!playerServiceLocator.TryGetServiceLocatorComponent(out _interactableDetector))return;
        
        var raycastHits = _interactableDetector.GetDetectedItemsSorted();

        if(raycastHits==null||raycastHits.Length==0)return;
        
        RichAI destSetter = FindObjectOfType<RichAI>();
        destSetter.destination = raycastHits[0].point;
    }
}
