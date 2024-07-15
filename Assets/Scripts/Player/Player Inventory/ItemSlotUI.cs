using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IServiceLocatorComponent
{
    public GameObject _slot;
	public GameObject _amountContainer;

	public UIImageMipmap _itemIcon;
	public Image _selectionImage;

	public TextMeshProUGUI _itemNameText;
	public TextMeshProUGUI _itemAmountText;
	public TextMeshProUGUI _slotNumberText;

	[SerializeField] private Animator _textAnimator;
	[SerializeField] private ItemData _emptyHand;

	private InventorySlot _inventorySlot;
	private ItemData _itemData;
	private bool _isSelected;

    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }


    public void SetInventorySlot(InventorySlot slot)
	{
		if(_inventorySlot != null)
        {
			UnsubscribeFromEvents();
        }

		_inventorySlot = slot;
		SubscribeOnEvents();
        SetSlotNumber(slot.IndexOfSlot);
		ClearPanel();
		SetHighlight(false);
		ItemAddedToEmptySlot(slot.ItemDataInSlot, 1);
	}

	private void ItemAddedToEmptySlot(ItemData inventoryItemData, int amount)
	{
		_itemData = inventoryItemData;

		if (_itemData == null) return;

        SetIcon(_itemData.Icon);

        SetAmountText(amount);

		if(_isSelected)
			SetItemNameText(_itemData?.GetItemName());
	}

	private void ClearPanel()
	{
		SetItemNameText(string.Empty);
        SetIcon(_emptyHand.Icon);
        _itemData = _emptyHand;
	}


	private void SubscribeOnEvents()
	{
		_inventorySlot.OnItemAmountChanged += SetAmountText;
		_inventorySlot.OnItemAdded += ItemAddedToEmptySlot;
		_inventorySlot.OnItemRemoved += ClearPanel;
		_inventorySlot.OnSlotSelected += SetHighlight;
	}

	private void UnsubscribeFromEvents()
	{
		if (_inventorySlot == null) return;
		_inventorySlot.OnItemAmountChanged -= SetAmountText;
		_inventorySlot.OnItemAdded -= ItemAddedToEmptySlot;
		_inventorySlot.OnItemRemoved -= ClearPanel;
		_inventorySlot.OnSlotSelected -= SetHighlight;
	}

	public void SetAmountText(int amount)
    {
		_amountContainer.SetActive(amount > 1);
		_itemAmountText.text = amount.ToString();
    }

	public void SetSlotNumber(int indexOfSlot)
	{
		if (!_inventorySlot.ShouldIndexSlots)
		{
			_slotNumberText.gameObject.SetActive(false);
			return;
		}

		_slotNumberText.gameObject.SetActive(true);
		if (indexOfSlot + 1 == 10)
			_slotNumberText.text = "0";
		else
			_slotNumberText.text = (indexOfSlot + 1).ToString();
	}

	public void SetItemNameText(string itemName)
	{
		if (_itemNameText == null /*|| _itemData == null || _itemData.Type == ItemType.EmptyHand*/) return;
		_itemNameText.text = itemName;

        if (itemName != string.Empty)
            _textAnimator.SetBool("Open", true);
        else
            _textAnimator.SetBool("Open", false);

        _textAnimator.SetTrigger("Play");

    }

	public void SetIcon(UIMipmap icon)
	{
		//_itemIcon.gameObject.SetActive(_itemData.Type != ItemType.EmptyHand);
		_itemIcon.SetMipmap(icon);
	}

	public void SetHighlight(bool isSelected) 
	{
		_isSelected = isSelected;

        _selectionImage.enabled = _isSelected;

		if (_itemData != null && _isSelected)
			SetItemNameText(_itemData.Type == ItemType.EmptyHand 
				? "" 
				: _itemData?.GetItemName());
	}

    public void DeleteWindow()
    {
    }

    public void OnDestroy()
    {
		UnsubscribeFromEvents();
	}
}
