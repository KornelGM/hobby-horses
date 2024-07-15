using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AItemContainer: MonoBehaviour, IServiceLocatorComponent, IStartable, ISaveable<IInventoryInfo>
{
    [ServiceLocatorComponent] protected NotificationsSystem _notificationsSystem;
    [ServiceLocatorComponent] protected ItemsManager _itemsManager;

    public virtual List<InventorySlot> GetSlots => _slotsList;
	public ServiceLocator MyServiceLocator { get; set; }
	public SlotsHelper SlotsHelper { get; set; }
			
	[SerializeField] private protected ServiceLocator _emptyHandServiceLocator;
	[SerializeField] private protected ItemData _emptyHand;
	[SerializeField] private bool _shouldIndexSlots = false;

	[Tooltip(" Used only in creating slot for chest's and in creation from context menu")]
	[SerializeField] [Min(0)] private protected int _amountOfSlots;

	protected List<InventorySlot> _slotsList = new List<InventorySlot>();
	
	public virtual void CustomStart()
	{
        CreateSlotsHelper();
    }

	public bool IsAnySlotEmpty()
	{
		foreach(InventorySlot slot in _slotsList)
		{
			if (slot.ItemDataInSlot == _emptyHand) return true;
		}

        //_notificationsSystem.SendSideNotification(TranslationKeys.NoSpaceInInventory, NotificationType.Warning);

        return false;
	}

	public virtual bool TryAddItemToContainer(ServiceLocator itemToAdd) => true;

	public virtual void RemoveFromSlot(int indexOfSlot)
	{
		_slotsList[indexOfSlot].RemoveItemFromSlot(amountOfItem: 1);
	}

	public virtual void RemoveFromSlot(ServiceLocator itemToRemove, int indexOfSlot)
	{
		InventorySlot slot = _slotsList[indexOfSlot];
		slot.RemoveItemFromSlot(itemToRemove, 1);
	}

	public virtual InventorySaveInfo GetInventoryInfo()
	{
		InventorySaveInfo inventoryInfo = new();

		foreach(InventorySlot slot in _slotsList)
		{
			inventoryInfo.InventoryItemSlotSaveInfos.Add(slot.CollectData(null));
        }

		return inventoryInfo;

    }

    public virtual void RemoveFromSlot(ServiceLocator itemToRemove) { }

	protected virtual void InitializeContainer()
	{
        CreateListOfSlots(_amountOfSlots);
    }

	protected virtual bool AddItemToFirstEmptySlot(ServiceLocator itemServiceLocator)
	{
		itemServiceLocator.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);
		
		InventorySlot firstEmptySlot = SlotsHelper.FindFirstEmptySlot(_slotsList);
		if (firstEmptySlot == null) return false;
		
		firstEmptySlot.TryAddItem(itemServiceLocator);
		
		return true;
	}

	protected virtual bool AddItemToSlotWithTeSameItem(ServiceLocator itemServiceLocator)
	{
		itemServiceLocator.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);

		InventorySlot inventorySlotWithSameItem = SlotsHelper.FindSlotWithSameItem(itemDataContainer.ItemData, _slotsList);
		if (inventorySlotWithSameItem == null) return false;
		
		inventorySlotWithSameItem.TryAddItem(itemServiceLocator);
		
		return true;
	}

	protected virtual void CreateListOfSlots(int amount)
	{
		if (_slotsList.Count >= _amountOfSlots)
			return;

		for (int i = 0; i < amount; i++)
		{
			InventorySlot inventorySlot = new InventorySlot(indexOfSlot: i, _emptyHand, 
				_emptyHandServiceLocator, _shouldIndexSlots, _itemsManager);

			_slotsList.Add(inventorySlot);
		}
	}
	protected bool TryAddItemToSlot(ServiceLocator itemServiceLocator, InventorySlot slot)
	{
		slot.TryAddItem(itemServiceLocator);
		return true;
	}

	public bool CheckItemIsInEquipment(ServiceLocator item)
	{
		ServiceLocator wantedItem = _slotsList.FirstOrDefault(x => x.ItemInSlotServiceLocator == item).
			ItemInSlotServiceLocator;

		return wantedItem != null;
    }

	private void CreateSlotsHelper() => 
		SlotsHelper = new SlotsHelper();

    public virtual IInventoryInfo CollectData(IInventoryInfo data)
    {
        foreach (InventorySlot slot in _slotsList)
        {
            ItemSlotSaveInfo slotInfo;
            slotInfo = slot.CollectData(null);
            data.InventoryItemSlotSaveInfos.Add(slotInfo);
        }

        return data;
    }

    public virtual void Initialize(IInventoryInfo save)
    {
        CreateListOfSlots(_amountOfSlots);
        if (save == null) return;

        for (int i = 0; i < save.InventoryItemSlotSaveInfos.Count; i++)
        {
            if (_slotsList.Count <= i) continue;
            _slotsList[i].Initialize(save.InventoryItemSlotSaveInfos[i]);
        }
    }
}