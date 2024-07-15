using UnityEngine;

public class SelectableItemsContainer : AItemContainer
{
	[ServiceLocatorComponent] protected CharacterHandInventorySlot _characterSlot;
    [ServiceLocatorComponent] protected HologramController _hologramController;
    [ServiceLocatorComponent] protected PlayerStateMachine _stateMachine;

    protected int _currentSelectedSlot = 0;

    void OnEnable()
    {
        _itemsManager.OnClientItemReceived += ClientReceiveItem;
    }
    void OnDisable() => _itemsManager.OnClientItemReceived -= ClientReceiveItem;

    private void ClientReceiveItem(int slotID, ItemServiceLocator item)
    {
        InventorySlot slot = _slotsList[slotID];
        TryAddItemToSlot(item, slot);
    }

    public override InventorySaveInfo GetInventoryInfo()
    {
        InventorySaveInfo inventoryInfo = base.GetInventoryInfo();

		inventoryInfo.SelectedSlot = _currentSelectedSlot;

        return inventoryInfo;

    }

    public override void CustomStart()
	{
		base.CustomStart();
		InitializeContainer();
    }

	public void ChangeToSlot(int slotIndex)
	{
        if (_hologramController && _hologramController.Projecting) return;

			PlayerMinigameState playerMinigameState =
		_stateMachine.CurrentState as PlayerMinigameState;

        if (playerMinigameState != null) return;

        if (_currentSelectedSlot >= 0 && _currentSelectedSlot < _slotsList.Count)
		{
			if (!_slotsList[_currentSelectedSlot].CanHideItem()) return;
			OnCurrentSlotDeselected();
		}

		OnSlotSelected(slotIndex);
	}

	public override bool TryAddItemToContainer(ServiceLocator itemToAdd)
	{
		if (TryAddItemToCurrentSlot(itemToAdd)) return true;
		return false;
	}

	public void RemoveFromCurrentSlot(ServiceLocator locator)
	{
		RemoveFromSlot(locator, _currentSelectedSlot);
	}

	protected virtual bool TryAddItemToCurrentSlot(ServiceLocator itemServiceLocator)
	{
		InventorySlot slot = _slotsList[_currentSelectedSlot];
		if (slot == null || !slot.IsEmpty) return false;

		return TryAddItemToSlot(itemServiceLocator, slot);
	}

	protected override bool AddItemToFirstEmptySlot(ServiceLocator itemServiceLocator)
	{
		InventorySlot slot = SlotsHelper.FindFirstEmptySlot(_slotsList);
		if (slot == null) return false;

        if (slot.IndexOfSlot != _currentSelectedSlot)
            itemServiceLocator.gameObject.SetActive(false);

        return TryAddItemToSlot(itemServiceLocator, slot);
	}

	private void OnSlotSelected(int slotIndex)
    {
		_currentSelectedSlot = slotIndex;
		InventorySlot slot = _slotsList[slotIndex];

		slot.OnItemAdded += SlotRefreshed;
		slot.OnSlotSelected += SlotRefreshed;
		slot.OnItemRemoved += SlotRefreshed;
		slot.OnItemMovedTo += OnItemMovedTo;

		slot.SlotSelected();
	}

	private void OnCurrentSlotDeselected()
	{
		InventorySlot previousSlot = _slotsList[_currentSelectedSlot];

		previousSlot.OnItemAdded -= SlotRefreshed;
		previousSlot.OnSlotSelected -= SlotRefreshed;
		previousSlot.OnItemRemoved -= SlotRefreshed;
		previousSlot.OnItemMovedTo -= OnItemMovedTo;

		previousSlot.SlotUnselected();
		ItemHidden(previousSlot);
	}

	public void ToggleItemOnCurrentSlot(bool toggle)
	{
		if (_slotsList[_currentSelectedSlot].ItemInSlotServiceLocator == null) return;

		GameObject itemToToggle = _slotsList[_currentSelectedSlot].ItemInSlotServiceLocator.gameObject;

		if (itemToToggle == null) return;

        itemToToggle.SetActive(toggle);
    }

	private void OnItemMovedTo(InventorySlot _, InventorySlot to) => ItemHidden(to);

	private void ItemHidden(InventorySlot slot)
	{
		if (slot.StuckItems.Count == 0) return;
		_characterSlot.OnItemHiddenInEQ?.Invoke(slot.StuckItems[0]);
	}

	private void SlotRefreshed() => SlotRefreshed(this);
	private void SlotRefreshed<T>(T arg1) => SlotRefreshed(arg1, arg1);
	private void SlotRefreshed<T, T2>(T arg1, T2 arg2)
	{
		_characterSlot.SlotRefreshed(_slotsList[_currentSelectedSlot]);
	}

    public override void Initialize(IInventoryInfo save)
    {
        base.Initialize(save);
        if (save == null)
        {
            return;
        }
		_currentSelectedSlot = save.SelectedSlot;
    }

    public override IInventoryInfo CollectData(IInventoryInfo data)
    {
        data = base.CollectData(data);
        data.SelectedSlot = _currentSelectedSlot;
        return data;
    }
}
