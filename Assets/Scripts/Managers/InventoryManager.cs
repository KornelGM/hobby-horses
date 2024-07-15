using UnityEngine;

public class InventoryManager : AItemContainer, IManager, IUpdateable, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private ItemDragHandler _itemDragHandler;

    [SerializeField] private PlayerInventory _inventoryPrefab;
    private PlayerInventory _inventory = null;
    private bool _opened = false;

    public bool Enabled => true;

    public override void CustomStart()
    {
        base.CustomStart();
        InitializeContainer();
    }

    public void CustomUpdate()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_opened) Close();
            else Open();
        }
    }

    public override bool TryAddItemToContainer(ServiceLocator itemToAdd)
    {
        if (base.TryAddItemToContainer(itemToAdd)) return true;
        if (AddItemToSlotWithTeSameItem(itemToAdd)) return true;
        if (AddItemToFirstEmptySlot(itemToAdd)) return true;
        Debug.Log("No Place in inventory");
        return false;
    }

    private void Open()
    {
        _opened = true;
        _inventory = _windowManager.CreateWindow(_inventoryPrefab, 1).GetComponent<PlayerInventory>();
        _inventory.Initialize(_slotsList);

        if (_playerManager.LocalPlayer &&
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerStateMachine stateMachine))
            stateMachine.SwitchState(new PlayerWaitState(stateMachine));
    }

    private void Close()
    {
        _opened = false;
        _windowManager.DeleteWindow(_inventory);

        if (_playerManager.LocalPlayer && _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerStateMachine stateMachine))
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));

        _itemDragHandler.Cancel();
    }

    public void CustomReset() { }
}