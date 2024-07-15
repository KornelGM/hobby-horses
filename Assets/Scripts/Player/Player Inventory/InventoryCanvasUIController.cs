using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class InventoryCanvasUIController : SerializedMonoBehaviour
{
	[HideInInspector] public bool IsInventoryOpened;
	[SerializeField] private GameObject _inventoryGameObject;
	[OdinSerialize] private IUIManager _uiManager;
	[OdinSerialize] private ITimeManager _timeManager;
	[OdinSerialize] private ICursorManager _cursorManager;

	private Player _player;
	private WaitForSeconds _cursorWait = new(1f);
	Button[] _childButtons;

	private void Awake()
	{
		_childButtons = transform.parent.GetComponentsInChildren<Button>();
	}

	public void Start()
	{
		_inventoryGameObject.IsNotNull(this, nameof(_inventoryGameObject));
		_uiManager.IsNotNull(this, nameof(_uiManager));
		_timeManager.IsNotNull(this, nameof(_timeManager));
		_cursorManager.IsNotNull(this, nameof(_cursorManager));

		_player = ReInput.players.GetPlayer(0);
		StartCoroutine(LateDisableCursor());
		DisableButtons();
	}

	private void Update()
	{
		if (_player.GetButtonDown(InputActionNames.Inventory))
			SetInventoryOpen(!_inventoryGameObject.activeSelf);
	}

	public void SetInventoryOpen(bool isOpen)
    {
		if (!IsInventoryOpened && _uiManager.IsOverlayUIOpen()) return;

		IsInventoryOpened = isOpen;
		_uiManager.EnableUIMap(IsInventoryOpened);
		_inventoryGameObject.SetActive(IsInventoryOpened);

		if (IsInventoryOpened)
		{
			_uiManager.AddOverlayUI(_inventoryGameObject);
			_timeManager.PauseGame();
		}
		else
		{
			_uiManager.RemoveOverlayUI(_inventoryGameObject);
			_timeManager.UnPauseGame();
		}
	}

	private IEnumerator LateDisableCursor()
	{
		yield return _cursorWait;
		_cursorManager.DeactivateCursor();
	}

	public void EnableButtons()
	{
		foreach (Button b in _childButtons)
		{
			b.interactable = true;

			if (b.TryGetComponent(out HoverUI hoverUI))
			{
				hoverUI.enabled = true;
			}

			if (b.TryGetComponent(out ItemSlotDragAndDrop slot))
			{
				slot.enabled = true;
			}
		}
	}

	public void DisableButtons()
	{
		foreach (Button b in _childButtons)
		{
			b.interactable = false;
			b.OnDeselect(null);

			if (b.TryGetComponent(out HoverUI hoverUI))
			{
				hoverUI.enabled = false;
				hoverUI.PerformActionOnPointerExit();
			}

			if (b.TryGetComponent(out ItemSlotDragAndDrop slot))
			{
				slot.enabled = false;
			}
		}
	}
}