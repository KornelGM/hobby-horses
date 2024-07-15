using System;
using System.Collections.Generic;

public interface IInventoryInfo
{
    public int SelectedSlot { get; set; }
    public List<ItemSlotSaveInfo> InventoryItemSlotSaveInfos { get; set; }
}

[Serializable]
public class InventorySaveInfo : IInventoryInfo
{
    public int SelectedSlot { get; set; } = 0;
    public List<ItemSlotSaveInfo> InventoryItemSlotSaveInfos { get; set; } = new();
}

[Serializable]
public class ItemSlotSaveInfo
{
    public List<string> Guids;
    public int SlotID;

    public ItemSlotSaveInfo(List<string> guidsInSlots, int slotID)
    {
        Guids = guidsInSlots;
        SlotID = slotID;
    }
}
