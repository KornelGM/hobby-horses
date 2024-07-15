using System.Collections.Generic;

public class SlotsHelper
{
	public InventorySlot FindFirstEmptySlot(List<InventorySlot> listOfSlots)
	{
		foreach (InventorySlot inventorySlot in listOfSlots) 
		{
			if (inventorySlot.IsEmpty) // here we also can check if slot contains EmptyHand item
				return inventorySlot;
		}
		return null;
	}

	public InventorySlot FindSlotWithSameItem(ItemData itemInSlot, List<InventorySlot> listOfSlots)
	{
		foreach (InventorySlot inventorySlot in listOfSlots)
		{
			if (inventorySlot.ItemDataInSlot != itemInSlot) continue;
			if (inventorySlot.IsFull) continue;
			return inventorySlot;
		}
		return null;
	}
}