using UnityEngine;

public class PlayerQuickAccessItemsBar : SelectableItemsContainer
{
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private WindowManager _windowManager;

    [SerializeField] private ItemSlotsVisualizer _itemSlotsVisualizerPrefab;

    public bool InvertInventory;
    private ItemSlotsVisualizer _itemSlotVisualiser;
    private IVirtualController _virtualController;
    public override void CustomStart()
	{
		base.CustomStart();
		InitializeInput();
        CheckFieldsNull();
        _itemSlotVisualiser = _windowManager.
			CreateWindow(_itemSlotsVisualizerPrefab).GetComponent<ItemSlotsVisualizer>();

		_itemSlotVisualiser?.ShowSlots(_slotsList);
        ChangeToSlot(_currentSelectedSlot);
    }

	private void InitializeInput()
	{
        if (_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _virtualController))
        {
			_virtualController.OnSetInventory0 += () => ChangeToSlot(9);
			_virtualController.OnSetInventory1 += () => ChangeToSlot(0);
			_virtualController.OnSetInventory2 += () => ChangeToSlot(1);
			_virtualController.OnSetInventory3 += () => ChangeToSlot(2);
			_virtualController.OnSetInventory4 += () => ChangeToSlot(3);
			_virtualController.OnSetInventory5 += () => ChangeToSlot(4);
			_virtualController.OnSetInventory6 += () => ChangeToSlot(5);
			_virtualController.OnSetInventory7 += () => ChangeToSlot(6);
			_virtualController.OnSetInventory8 += () => ChangeToSlot(7);
			_virtualController.OnSetInventory9 += () => ChangeToSlot(8);
			_virtualController.OnScrollInventoryUp += () => ChangeSlotBy(-1);
			_virtualController.OnScrollInventoryDown += () => ChangeSlotBy(1);
        }
    }

    private void OnDestroy()
    {
        if (_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _virtualController))
        {
            _virtualController.OnSetInventory0 -= () => ChangeToSlot(9);
            _virtualController.OnSetInventory1 -= () => ChangeToSlot(0);
            _virtualController.OnSetInventory2 -= () => ChangeToSlot(1);
            _virtualController.OnSetInventory3 -= () => ChangeToSlot(2);
            _virtualController.OnSetInventory4 -= () => ChangeToSlot(3);
            _virtualController.OnSetInventory5 -= () => ChangeToSlot(4);
            _virtualController.OnSetInventory6 -= () => ChangeToSlot(5);
            _virtualController.OnSetInventory7 -= () => ChangeToSlot(6);
            _virtualController.OnSetInventory8 -= () => ChangeToSlot(7);
            _virtualController.OnSetInventory9 -= () => ChangeToSlot(8);
            _virtualController.OnScrollInventoryUp -= () => ChangeSlotBy(-1);
            _virtualController.OnScrollInventoryDown -= () => ChangeSlotBy(1);
        }
    }

	private void ChangeSlotBy(int diff)
    {
        if (_hologramController && _hologramController.Projecting) return;

        PlayerMinigameState playerMinigameState =
            _stateMachine.CurrentState as PlayerMinigameState;

        if (playerMinigameState != null) return;

        int newSlot = InvertInventory ? _currentSelectedSlot - diff : _currentSelectedSlot + diff;
		if (newSlot < 0) newSlot += _amountOfSlots;
		newSlot = newSlot % _amountOfSlots;

		ChangeToSlot(newSlot);
	}

    public override bool TryAddItemToContainer(ServiceLocator itemToAdd)
    {
        if (TryAddItemToCurrentSlot(itemToAdd)) return true;
        if (AddItemToFirstEmptySlot(itemToAdd)) return true;
		
        return false;
    }

    protected override bool AddItemToSlotWithTeSameItem(ServiceLocator itemServiceLocator)
	{
		itemServiceLocator.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);
		InventorySlot slot = SlotsHelper.FindSlotWithSameItem(itemDataContainer.ItemData, _slotsList);
		if (slot == null) return false;

		return TryAddItemToSlot(itemServiceLocator, slot);
	}

    public void ToggleItemSlotVisualiser(bool toggle)
    {
        if (_itemSlotVisualiser == null) return;

        _itemSlotVisualiser.MyObject.transform.localScale = toggle ? Vector3.one : Vector3.zero;
    }

	private void CheckFieldsNull()
	{
		_emptyHand.IsNotNull(this, nameof(_emptyHand));
	}
}