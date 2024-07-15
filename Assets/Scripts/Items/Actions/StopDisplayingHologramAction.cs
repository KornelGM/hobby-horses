public class StopDisplayingHologramAction : BaseAction
{
    public override bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand)
    {
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out HologramController hologramController)) return;

        hologramController.StopDisplayingHologram();
    }
}