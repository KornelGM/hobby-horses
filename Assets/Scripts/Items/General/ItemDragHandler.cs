using UnityEngine;
using Rewired;

public class ItemDragHandler : MonoBehaviour, IManager, IStartable, IUpdateable, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [SerializeField] private ServiceLocator _emptyHandServiceLocator;
    [SerializeField] private ItemSlotUI _slotPrefab;

    private ItemSlotUI _slot;

    private InventorySlot _onOver = null;
    private InventorySlot _origin = null;
    private InventorySlot _mySlot = null;

    private Player _player;

    public bool Enabled => true;

    public void CustomStart()
    {
        _slot = Instantiate(_slotPrefab,_windowManager.MainCanvas.transform).GetComponent<ItemSlotUI>();
        _player = ReInput.players.GetPlayer(0);
        _slot.gameObject.SetActive(false);
        _mySlot = null;
    }

    public void CustomUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(_onOver);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleClick(_onOver);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CheckDragAndDrop(_onOver);
        }

        _slot.transform.position = _player.controllers.Mouse.screenPosition;
    }

    public void OnOver(InventorySlot slot)
    {
        _onOver = slot;
    }

    public void HandleClick(InventorySlot slot)
    {
        if (slot == null) return;
        if (slot.IsEmpty) return;
        if (!slot.CanHideItem()) return;

        if (_mySlot == null)
        {
            _origin = slot;
            _mySlot = new InventorySlot();
            _slot.SetInventorySlot(_mySlot);
            TransferData(slot, _mySlot);
            _slot.gameObject.SetActive(true);
        }
    }

    public void Cancel()
    {
        CheckDragAndDrop(null);

        _slot.gameObject.SetActive(false);
        _origin = null;
        _mySlot = null;
        _onOver = null;
    }

    public void CheckDragAndDrop(InventorySlot target)
    {
        if (_mySlot == null) return;

        if (target == null || !target.CanHideItem())
        {
            TransferData(_mySlot, _origin);
        }
        else
        {
            if(TransferData(target, _origin))
                TransferData(_mySlot, target);
            else TransferData(_mySlot, _origin);
        }
        _slot.gameObject.SetActive(false);
        _mySlot = null;
        _origin = null;   
    }

    private bool TransferData(InventorySlot origin, InventorySlot target)
    {
        if(origin == null) return false;
        if(target == null) return false;

        if (!target.TryAddItem(origin.StuckItems)) return false;
        origin.RemoveItemFromSlot(origin.CurrentAmountOfItemInSlot);
        origin.ItemMovedTo(target);
        return true;
    }

    public void CustomReset()
    {
        
    }
}
