using System;

public class CharacterHandInventorySlot: IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public ServiceLocator CurrentItem { get; private set; }
    public Action<ServiceLocator> OnItemRefreshed;
    public Action<ServiceLocator> OnItemHiddenInEQ;
    public void SlotRefreshed(InventorySlot slot)
    {
        if(slot.StuckItems.Count == 0)
        {
            CurrentItem = null;
            OnItemRefreshed?.Invoke(null);
            return;
        }

        ServiceLocator item = slot.StuckItems[0];
        CurrentItem = item;
        OnItemRefreshed?.Invoke(item);
    }
}
