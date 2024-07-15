using System.Collections.Generic;

public class GlobalChest : AChest
{
	public GlobalSlotsContainer GlobalSlotsContainer;

	public override List<InventorySlot> GetSlots =>
		GlobalSlotsContainer.GetSlots;
}