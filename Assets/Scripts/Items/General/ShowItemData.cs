using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class ShowItemData : MonoBehaviour, IServiceLocatorComponent, IStartable
{
	public ServiceLocator MyServiceLocator { get; set; }

	[ServiceLocatorComponent] private ItemInteractionTooltipManager _tooltipManager;
	[ServiceLocatorComponent] private VisualItemService _visuals;
	[ServiceLocatorComponent] private ItemDataContainer _itemDataContainer;

    [CanBeNull]
	[ServiceLocatorComponent] private ITooltipPoint _toolTipPoint;
	private IAvailableActions _availableActions;


	public void CustomStart()
	{
		_visuals.ToggleOutlines(false);
	}

	public void SetupActionsReference(IAvailableActions availableActions)
    {
		_availableActions = availableActions;
		
		foreach (IAction availableAction in _availableActions.CurrentActions.Values)
		{
			availableAction?.OnStart();
		}
		
		_availableActions.OnCurrentActionChanged += ShowAction;
		ShowTooltip(_availableActions.CurrentActions);
	}

	public void CleanupAvailableActions()
    {
		if (_availableActions == null) return;
		
		foreach (IAction availableAction in _availableActions.CurrentActions.Values)
		{
			availableAction?.OnStop();
		}

		_availableActions.OnCurrentActionChanged -= ShowAction;
		_availableActions = null;
    }

	private void ShowAction(InteractionType type, IAction action)
    {
		if (_availableActions == null) return;
		IActionTooltip mainTooltip = ActionsUtility.GetMainTooltipOnObject(_availableActions.CurrentActions);

		if(action == null)
		{
			_tooltipManager.RefreshAction(_toolTipPoint, type, null);
			return;
        }

		if(action.Tooltip != null)
		{
            if (action.Tooltip.ShowOnObject)
            {
                _tooltipManager.HideScreenAction(type);
                _tooltipManager.RefreshAction(_toolTipPoint, type, action?.Tooltip);
            }
            else
            {
                _tooltipManager.HideAction(_toolTipPoint, type);
                _tooltipManager.RefreshScreenAction(type, action?.Tooltip);
            }
        }

		if (mainTooltip != null)
		{
			_visuals.ToggleOutlines(true);
			_visuals.ToggleOutlineColor(mainTooltip.OutlineColor);
		}
	}

	private void ShowTooltip(Dictionary<InteractionType, IAction> actions)
	{
		if (!ActionsUtility.TryGetTooltipsFromAvailableActions
			(actions, 
			out Dictionary<InteractionType, IActionTooltip> tooltipsOnObject,
			out Dictionary<InteractionType, IActionTooltip> tooltipsOnScreen,
			out IActionTooltip mainTooltip)) return;

		ClampedValue progressBar = GetProgressBar(actions.Values.ToList());

		ItemInteractionTooltip objectTooltip = new ItemInteractionTooltip(_itemDataContainer.ItemData.GetItemName(), tooltipsOnObject);
		ItemInteractionTooltip screenTooltip = new ItemInteractionTooltip("", tooltipsOnScreen);

		_tooltipManager.ShowTooltip(_toolTipPoint, objectTooltip);
		_tooltipManager.ShowTooltipOnScreen(screenTooltip);

        if (MyServiceLocator.TryGetAllServiceLocatorComponentsOfType(out List<ShowAdditionalInfo> additionals, true))
        {
            ShowAdditionalInfo additional = additionals.FirstOrDefault(item => item.ShowOnFocus == true);

            if (additional != null)
			{
				additional.ShowInfo();
			}
        }

        if (mainTooltip != null)
		{
			_visuals.ToggleOutlines(true);
			_visuals.ToggleOutlineColor(mainTooltip.OutlineColor);
		}
	}

	void OnDestroy()
    {
		if(_availableActions != null)HideTooltip();
	}

	public void HideTooltip()
    {
		CleanupAvailableActions();

        if (MyServiceLocator.TryGetAllServiceLocatorComponentsOfType(out List<ShowAdditionalInfo> additionals, true))
        {
			foreach (ShowAdditionalInfo additional in additionals)
			{
				additional.HideInfo();
			}
        }

        _tooltipManager.HideTooltip(_toolTipPoint);
		_tooltipManager.HideTooltipOnScreen();
		_visuals.ToggleOutlines(false);
	}

	private ClampedValue GetProgressBar(List<IAction> actions)
	{
		foreach (IAction action in actions)
		{
			if (action == null) continue;
			if (action.ProgressBar == null) continue;
			return action.ProgressBar;
		}
		return null;
	}

}

