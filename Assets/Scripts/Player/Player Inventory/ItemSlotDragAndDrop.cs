using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;

public class ItemSlotDragAndDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IServiceLocatorComponent
{
	[ServiceLocatorComponent] private ItemDragHandler _dragHandler;

	private InventorySlot _inventorySlot;
    public ServiceLocator MyServiceLocator { get; set; }


    public void SetInventorySlot(InventorySlot inventorySlot)
	{
		_inventorySlot = inventorySlot;
	}

	public void OnPointerEnter(PointerEventData eventData) => _dragHandler.OnOver(_inventorySlot);
	public void OnPointerExit(PointerEventData eventData) => _dragHandler.OnOver(null);
}