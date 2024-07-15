public class ItemReplacer
{
	public bool ReplaceItem(ServiceLocator item, InventorySlot previousItemInventorySlot, InventorySlot inventorySlotWhereItemMustBePlaced)
	{
		item.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);
		
		if (inventorySlotWhereItemMustBePlaced.ItemDataInSlot == itemDataContainer.ItemData)
		{
			return CanBeStacked(previousItemInventorySlot, inventorySlotWhereItemMustBePlaced)
				?
				ReplaceItemToSlotWithTheSameItem(item, previousItemInventorySlot, inventorySlotWhereItemMustBePlaced)
				:
				ReplaceItemToSlotWithDifferentItem(item, previousItemInventorySlot, inventorySlotWhereItemMustBePlaced);
		}

		if (!inventorySlotWhereItemMustBePlaced.IsEmpty)
		{
			return ReplaceItemToSlotWithDifferentItem(item, previousItemInventorySlot, inventorySlotWhereItemMustBePlaced);
		}
		
		inventorySlotWhereItemMustBePlaced.TryAddItem(item);
		previousItemInventorySlot.RemoveItemFromSlot(previousItemInventorySlot.CurrentAmountOfItemInSlot);

		return true;
	}

	private bool ReplaceItemToSlotWithTheSameItem(ServiceLocator item, InventorySlot previousItemInventorySlot, InventorySlot inventorySlotWhereItemMustBePlaced)
	{
		if (!inventorySlotWhereItemMustBePlaced.TryAddItem(item))
			return false;
		
		previousItemInventorySlot.RemoveItemFromSlot(previousItemInventorySlot.CurrentAmountOfItemInSlot);

		return true;
	}

	private bool ReplaceItemToSlotWithDifferentItem(ServiceLocator item, InventorySlot previousItemInventorySlot, InventorySlot inventorySlotWhereItemMustBePlaced)
	{
		int amountOfItemInPreviousSlot = previousItemInventorySlot.CurrentAmountOfItemInSlot;
		ServiceLocator itemInPreviousSlot = item;
		
		int amountOfItemSlotWhereItemMustBePlaced = inventorySlotWhereItemMustBePlaced.CurrentAmountOfItemInSlot;
		ServiceLocator itemInSlotWhereItemMustBePlaced = inventorySlotWhereItemMustBePlaced.ItemInSlotServiceLocator;
		
		previousItemInventorySlot.RemoveItemFromSlot(previousItemInventorySlot.CurrentAmountOfItemInSlot);
		inventorySlotWhereItemMustBePlaced.RemoveItemFromSlot(inventorySlotWhereItemMustBePlaced.CurrentAmountOfItemInSlot);
		
		if(!previousItemInventorySlot.TryAddItem(itemInSlotWhereItemMustBePlaced))
			return false;

		if (!inventorySlotWhereItemMustBePlaced.TryAddItem(itemInPreviousSlot))
			return false;
		
		return true;
	}

	private bool CanBeStacked(InventorySlot previousInventorySlot, InventorySlot nextInventorySlot) => 
		previousInventorySlot.CurrentAmountOfItemInSlot + nextInventorySlot.CurrentAmountOfItemInSlot <= nextInventorySlot.MaxAmountOfItemInSlot;
}
