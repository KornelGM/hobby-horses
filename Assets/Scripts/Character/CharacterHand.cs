using System;

public class CharacterHand : Hand
{
	[ServiceLocatorComponent] private CharacterHandInventorySlot _selectedSlot;

	public event Action<ItemData> OnItemPlacedInHand;	

	public override void CustomAwake()
	{
        base.CustomAwake();
        
        SubscribeToEvents();
    }

    public override void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        OnItemInSlotRefresh(_selectedSlot.CurrentItem);
        
        _selectedSlot.OnItemRefreshed += OnItemInSlotRefresh;
        _selectedSlot.OnItemHiddenInEQ += OnItemHidden;
    }

    private void UnsubscribeFromEvents()
    {
        ChangeItemInHand(null);
        
        _selectedSlot.OnItemRefreshed -= OnItemInSlotRefresh;
        _selectedSlot.OnItemHiddenInEQ -= OnItemHidden;
    }

    protected override void OnNewItemPlacedInHand(ServiceLocator item)
    {
        base.OnNewItemPlacedInHand(item);
        
        OnItemPlacedInHand?.Invoke(ItemDataInItemInHand);
    }
}
