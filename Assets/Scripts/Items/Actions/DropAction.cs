using UnityEngine.Events;
using UnityEngine;
using Sirenix.OdinInspector;

public class DropAction : BaseAction, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField, FoldoutGroup("Audio")] private AudioPlayer _audioPlayer;
    [SerializeField, FoldoutGroup("Audio")] private AudioStorage _audioStorage;
    [SerializeField, FoldoutGroup("Audio")] private AudioSource _audioSource;

    public UnityEvent OnItemDropped;
    [Space(10)]
    public UnityEvent OnEndDropAnimation;

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, 
        ServiceLocator caller) => true;
    
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, 
        ServiceLocator itemInHand)
    {
        if (itemInHand == null) return;
        
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out Hand hand)) return;
        
        if (hand.ItemInHand != itemInHand) return;

        if (playerServiceLocator.TryGetServiceLocatorComponent(out SelectableItemsContainer quickAccessItemsBar, allowToBeNull: true))
        {
            quickAccessItemsBar.RemoveFromCurrentSlot(itemInHand);
        }

        if (itemInHand.TryGetServiceLocatorComponent(out VisualItemService visualItemService, true, true))
        {
            ChangeVisualsLayer(visualItemService);
        }

        Drop(itemInHand.gameObject);
        OnItemDropped?.Invoke();
        
        base.Perform(playerServiceLocator, itemInInteractionServiceLocator, itemInHand);
    }

    private void ChangeVisualsLayer(VisualItemService visualItemService)
    {
        Transform[] children = visualItemService.gameObject.GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child.GetComponent<DontChangeLayerOnThisItem>())
                continue;

            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void Drop(GameObject objectToDrop)
    {
        if (objectToDrop.layer != LayerMask.NameToLayer("Default")) return;
        if (!objectToDrop.TryGetComponent(out Rigidbody rigidBody)) return;
        
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
    }

    public void PlayDropSound()
    {
        if (_audioPlayer == null || _audioStorage == null || _audioSource == null) return;

        _audioPlayer.PlayEvent(_audioStorage.GetRandomAudioEventVariant(), _audioSource);
    }
}
