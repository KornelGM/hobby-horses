using UnityEngine;

public class SpawnObjectInHandAction : BaseAction, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private ItemsManager itemsManager;
    [SerializeField] private ItemData _item;
    [SerializeField] private PickupAction _pickup;
    public ServiceLocator MyServiceLocator { get; set; }

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator itemInHand) => TryPickup(playerServiceLocator);

    private void TryPickup(ServiceLocator playerServiceLocator)
    {
        if (playerServiceLocator == null) return;

        ServiceLocator serviceLocator = itemsManager.SpawnItem(_item, transform.position, Quaternion.identity);
        _pickup.Perform(playerServiceLocator, serviceLocator, null);
    }
}
