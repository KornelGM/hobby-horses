using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IWindow
{
	[SerializeField] private ItemSlotsVisualizer _itemSlotsVisualizer;

    public WindowManager Manager { get; set; }

    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = false;

    public void Initialize(List<InventorySlot> slots)
    {
        _itemSlotsVisualizer?.ShowSlots(slots);
    }

    public void OnDeleteWindow()
    {
        _itemSlotsVisualizer?.HideSlots();
    }

}
