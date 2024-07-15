namespace Items.Actions
{
    public class CancelDisplayingHologramAction : BaseAction
    {
        public override bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
        public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
        {
            if (!playerServiceLocator.TryGetServiceLocatorComponent(out HologramController hologramController)) return;
            hologramController.CancelDisplayingHologram();
        }
    }
}