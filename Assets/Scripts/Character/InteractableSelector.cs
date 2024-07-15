using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class InteractableSelector : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }
    
	[ServiceLocatorComponent] private PlayerCurrentActionsReference _availableActions;
	[ServiceLocatorComponent] private InteractableDetector _interactableDetector;
	[ServiceLocatorComponent] private CharacterHand _playerHand;
	[ServiceLocatorComponent] private ItemInteractionTooltipManager _tooltipManager;

	private List<object> _blockers = new();
	public bool Blocked => _blocked;
	private bool _blocked => _blockers.Count > 0;

	public ServiceLocator ItemToInteract
    {
        get
        {
			if (_detectedItem == null) return null;
			return _detectedItem.MyServiceLocator;
        }
    }

	private ItemDetectable _detectedItem;

	public void CustomAwake()
	{
		_playerHand.OnItemPlacedInHand += (action) => RefreshFocus();
		_availableActions.OnCurrentActionChanged += RefreshTooltips;
		//_availableActions.OnAllActionsChanged += ShowHeldObjectTooltip;
	}

	void OnDestroy()
    {
		_playerHand.OnItemPlacedInHand -= (action) => RefreshFocus();
		_availableActions.OnCurrentActionChanged -= RefreshTooltips;
		//_availableActions.OnAllActionsChanged -= ShowHeldObjectTooltip;
    }

	public void Block(object caller)
    {
        _availableActions.CancelAllActions();
		if (_blockers.Contains(caller)) return;
		_blockers.Add(caller);
		Deselect();
    }

	public void Unblock(object caller)
    {
		_blockers.Remove(caller);
    }

	public void DetectItems(bool force = false)
	{
		if (_blocked) 
			return;

		RaycastHit[] detectedObjects = _interactableDetector.GetDetectedItemsSorted();
		if (detectedObjects == null || detectedObjects.Length == 0)
		{
			if (_detectedItem == null) return;
			Deselect();
			return;
		}

		CheckSelectedItem(detectedObjects[0].collider.gameObject, force);
	}

	public void SetupActionContainers()
	{
		_availableActions.SetupActions(_playerHand.ItemInHand, _detectedItem?.MyServiceLocator);
	}

	private void CheckSelectedItem(GameObject obj, bool force)
    {
		ItemDetectable itemDetectable;
		if(	obj == null ||
			!obj.TryGetComponent(out itemDetectable)||
			itemDetectable == null || 
			!itemDetectable.AbleToDetect)
        {
			if (_detectedItem == null) return;
			Deselect();
			return;
        }

		SelectItem(itemDetectable, force);
    }

	private void SelectItem(ItemDetectable detectable, bool force)
    {
		if (DetectedSameObject() && !force) return;
		if(_playerHand.ItemInHand == null)_playerHand.ItemInHandIsNull();

		Deselect();
		_detectedItem = detectable;

		SetupActionContainers();
		_detectedItem.ShowItemData.SetupActionsReference(_availableActions);

		bool DetectedSameObject() => detectable == _detectedItem;
    }

	public void RefreshFocus()
    {
		Deselect(true);
		DetectItems(true);
	}

	public void Deselect(bool refresh = false)
	{
		if (_playerHand.ItemInHand == null)_playerHand.ItemInHandIsNull();

		if (_detectedItem != null)
        {
			if(!refresh)
			{
                _detectedItem.ShowItemData.HideTooltip();
                _detectedItem = null;
            }
            else
            {
                _detectedItem.ShowItemData.CleanupAvailableActions();
            }
        }

		SetupActionContainers();
		ShowHeldObjectTooltip(_availableActions.CurrentActions);
	}

	private void RefreshTooltips(InteractionType type, IAction action)
    {
		if (_detectedItem == null)
			_tooltipManager.RefreshScreenAction(type, action?.Tooltip);
	}

	private void ShowHeldObjectTooltip(Dictionary<InteractionType, IAction> actions)
	{
		if (_detectedItem != null) return;

		_tooltipManager.HideTooltipOnScreen();

		if (!ActionsUtility.TryGetTooltipsFromAvailableActions
			(actions, out Dictionary<InteractionType, IActionTooltip> tooltipsOnScreen)) return;

		ItemInteractionTooltip screenTooltip = new ItemInteractionTooltip("", tooltipsOnScreen);
		_tooltipManager.ShowTooltipOnScreen(screenTooltip);
	}
}
