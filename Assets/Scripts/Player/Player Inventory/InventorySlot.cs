using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot: ISaveable<ItemSlotSaveInfo>
{
	public bool IsFull => _currentAmountOfItemInSlot >= _maxAmountOfItemInSlot;
	public bool IsEmpty => _currentAmountOfItemInSlot <= 0;
	public bool ShouldIndexSlots;
	public int MaxAmountOfItemInSlot => _maxAmountOfItemInSlot;
	public int CurrentAmountOfItemInSlot => _currentAmountOfItemInSlot;

	public event Action<ItemData, int> OnItemAdded;
	public event Action OnItemRemoved;
	public event Action<InventorySlot, InventorySlot> OnItemMovedTo;
	public event Action<int> OnItemAmountChanged;
	public event Action<bool> OnSlotSelected;

	public List<ServiceLocator> StuckItems = new List<ServiceLocator>();
	public ItemData ItemDataInSlot = null;
	public ServiceLocator ItemInSlotServiceLocator;
	public int IndexOfSlot = -1;
	
	private ItemData _emptyHand;
	private ServiceLocator _emptyHandServiceLocator;
	private int _maxAmountOfItemInSlot = 3;
	private int _currentAmountOfItemInSlot = 0;

    private ItemsManager _itemsManager;

    public InventorySlot(int indexOfSlot, ItemData emptyHand, ServiceLocator emptyHandServiceLocator, 
		bool shouldIndexSlots, ItemsManager itemsManager)
    {
		ShouldIndexSlots = shouldIndexSlots;
		ItemDataInSlot = emptyHand;
		_emptyHand = emptyHand;
		ItemInSlotServiceLocator = emptyHandServiceLocator;
		_emptyHandServiceLocator = emptyHandServiceLocator;
		IndexOfSlot = indexOfSlot;
        _itemsManager = itemsManager;
    }

	public InventorySlot(){}

	public bool TryAddItem(ServiceLocator newItem) => TryAddItem(new List<ServiceLocator>() {newItem});

	public bool TryAddItem(List<ServiceLocator> newItems)
	{
		if (newItems.Count == 0) return true;
		if (_currentAmountOfItemInSlot + newItems.Count > _maxAmountOfItemInSlot) return false;

		StuckItems.AddRange(newItems);

		if (IsEmpty) AddNewItemToSlot(newItems[0], newItems.Count);
		else UpdateAmount(newItems.Count);

		return true;
	}

	public void RemoveItemFromSlot(int amountOfItem)
	{
		if (IsEmpty) return;
		
		UpdateAmount(-amountOfItem);
		StuckItems.RemoveAt(StuckItems.Count - 1);

		if (_currentAmountOfItemInSlot == 0) ClearSlot();
		OnItemRemoved?.Invoke();
	}

	public void RemoveItemFromSlot(ServiceLocator item, int amountOfItem)
	{
		if (IsEmpty) return;

		StuckItems.Remove(item);
		UpdateAmount(-amountOfItem);	

		if (StuckItems.Count == 0) ClearSlot();
		OnItemRemoved?.Invoke();
	}

	public void SlotSelected() => OnSlotSelected?.Invoke(true);
	public void SlotUnselected() => OnSlotSelected?.Invoke(false);

	public void ClearSlot()
	{
		_currentAmountOfItemInSlot = 0;
		ItemDataInSlot = _emptyHand;
		ItemInSlotServiceLocator = _emptyHandServiceLocator;
		StuckItems.Clear();
		
		OnItemRemoved?.Invoke();
	}

	public void ItemMovedTo(InventorySlot slot)
    {
		OnItemMovedTo?.Invoke(this,slot);
    }

	private void UpdateAmount(int amount)
	{
		_currentAmountOfItemInSlot += amount;	
		_currentAmountOfItemInSlot = Mathf.Clamp(_currentAmountOfItemInSlot, 0, _maxAmountOfItemInSlot);
		
		OnItemAmountChanged?.Invoke(_currentAmountOfItemInSlot);
	}

	private void AddNewItemToSlot(ServiceLocator itemToAddInSlot, int amount)
	{
		itemToAddInSlot.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);	
		ItemInSlotServiceLocator = itemToAddInSlot;
		ItemDataInSlot = itemDataContainer.ItemData;

        UpdateAmount(amount);
        ItemSlotSaveInfo itemSlotSaveInfo = CollectData(null);

        OnItemAdded?.Invoke(ItemDataInSlot, amount);
	}

	public bool CanHideItem()
	{
		if (ItemDataInSlot == null) return false;
		return ItemDataInSlot.AbleToChangeSlot;
	}

    public ItemSlotSaveInfo CollectData(ItemSlotSaveInfo data)
    {
        List<string> guids = new();

        foreach (ServiceLocator item in StuckItems)
        {
            string guid = GetGuid(item);
            if (string.IsNullOrEmpty(guid))
                continue;

            guids.Add(guid);
        }
        return new(guids, IndexOfSlot);
    }

    private string GetGuid(ServiceLocator serviceLocator)
    {
        ItemServiceLocator item = serviceLocator as ItemServiceLocator;
        if (item == null) return string.Empty;
        if (!item.TryGetServiceLocatorComponent(out ItemSaveService saveService)) return string.Empty;

        return saveService.GUID;
    }

    public void Initialize(ItemSlotSaveInfo save)
    {
        //foreach (string guid in save.Guids)
        //{
        //    if (!_itemsManager.GetItemByGUID(guid, out ItemServiceLocator item))
        //    {
        //        Debug.LogError($"Couldn't find an object with guid {guid}");
        //        continue;
        //    }

        //    TryAddItem(item);
        //}
    }
}