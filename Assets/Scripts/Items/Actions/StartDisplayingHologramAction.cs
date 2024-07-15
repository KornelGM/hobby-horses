using UnityEngine;

public class StartDisplayingHologramAction : BaseAction
{
    [SerializeField] private ItemHologramSettings itemHologramSettings = null;
    public override bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand)
    {
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out HologramController hologramController)) return;
        if (!itemInHand.TryGetServiceLocatorComponent(out VisualItemService visualItemService)) return;

        hologramController.StartDisplaying(visualItemService.Model, itemHologramSettings);
    }
}