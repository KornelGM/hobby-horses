using DG.Tweening;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class PickupAction : BaseAction, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public UnityEvent<PickupAction> OnItemPickup;

    [ServiceLocatorComponent] private VisualItemService _visualItemService;
    [ServiceLocatorComponent] private StatsManager _statsManager;

    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private AudioStorage _audioStorage;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ActionStat _pickupAction;

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    {
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out PickingUpItemControllerForPlayer pickingUpItemController))
            return false;

        if (!pickingUpItemController.QuickAccessItemBar.IsAnySlotEmpty())
        {
            return false;
        }

        return true;
    }

    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    {
        base.Perform(playerServiceLocator, interactionItem, caller);
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out PickingUpItemController pickingUpItemController)) return;

        if (pickingUpItemController.CantPickUp)
            return;

        interactionItem.transform.DOKill();
        pickingUpItemController.StartPickingUpSmoothed(interactionItem);

        HandleRigidbody(interactionItem.gameObject);
        PlayPickUpSoundRpc();
        InvokePickupActionStat(interactionItem);
        OnItemPickup?.Invoke(this);

        if (interactionItem.TryGetServiceLocatorComponent(out VisualItemService visualItemService, true, true))
        {
            ChangeVisualsLayer(visualItemService);
        }

        InvokeActionFinished();
    }

    private void InvokePickupActionStat(ItemData item, string guid)
    {
        ItemDataAndGuidActionStatData pickupItem = new ItemDataAndGuidActionStatData(item.Guid, guid);
        _statsManager.AddStat(_pickupAction, pickupItem);
    }

    private void InvokePickupActionStat(ServiceLocator interactionItem)
    {
        if(interactionItem.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer, true, false))
        {
            if (interactionItem.TryGetServiceLocatorComponent(out ItemSaveService saveService, true, false))
            {
                InvokePickupActionStat(itemDataContainer.ItemData, saveService.GUID);
            }
        }
    }

    private void HandleRigidbody(GameObject obj)
    {
        Rigidbody rb;
        if (obj.TryGetComponent(out rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    private void DisableVisualsColliders()
    {
        if (_visualItemService == null) return;
        Collider[] collidersToDisable = _visualItemService.Model.transform.GetComponentsInChildren<Collider>();

        foreach (var collider in collidersToDisable)
        {
            collider.enabled = false;
        }
    }

    private void ChangeVisualsLayer(VisualItemService visualItemService)
    {
        Transform[] children = visualItemService.gameObject.GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child.GetComponent<DontChangeLayerOnThisItem>())
                continue;

            child.gameObject.layer = LayerMask.NameToLayer("Hand");
        }
       
    }

    private void PlayPickUpSoundRpc()
    {
        if (_audioPlayer == null || _audioStorage == null || _audioSource == null) return;

        _audioPlayer.PlayEvent(_audioStorage.GetRandomAudioEventVariant(), _audioSource);
    }
}
