using System.Collections.Generic;
using UnityEngine;

public class ItemSlotsVisualizer : MonoBehaviour, IWindow
{
    public WindowManager Manager { get; set; }

    public GameObject MyObject => gameObject;

    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; } = true;
    public bool ShouldActivateCursor { get; set; } = false;
    public bool ShouldDeactivateCrosshair { get; set; } = false;

    [SerializeField] private ServiceLocator _panelPrefab;
    [SerializeField] private Transform _layout;
    [SerializeField] private List<ServiceLocator> _predefinedPanels = new List<ServiceLocator>();

    private List<ServiceLocator> _panels = new List<ServiceLocator>();

    public void ShowSlots(List<InventorySlot> slots)
    {
        if(_predefinedPanels.Count > 0)
        {
            for (int i = 0; i < _predefinedPanels.Count; i++)
            {
                ServiceLocator panel = _predefinedPanels[i];
                InitializePanel(panel, slots[i]);
            }
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            CreatePanel(slots[i], _layout);
        }
    }

    public void HideSlots()
    {
        for (int i = _panels.Count - 1; i >= 0; i--)
        {
            ServiceLocator panel = _panels[i];
            _panels.RemoveAt(i);
            Destroy(panel.gameObject);
        }
    }

    private void CreatePanel(InventorySlot inventorySlot, Transform layout)
    {
        ServiceLocator slotPanel = Instantiate(_panelPrefab, layout);
        slotPanel.CustomAwake();

        InitializePanel(slotPanel, inventorySlot);
        _panels.Add(slotPanel);
    }

    private void InitializePanel(ServiceLocator slotPanel, InventorySlot inventorySlot)
    {
        slotPanel.TryGetServiceLocatorComponent(out ItemSlotUI slotUIComponentsHolder);
        slotPanel.TryGetServiceLocatorComponent(out ItemSlotDragAndDrop itemSlotDragAndDrop);

        itemSlotDragAndDrop.SetInventorySlot(inventorySlot);
        slotUIComponentsHolder.SetInventorySlot(inventorySlot);
    }

}
