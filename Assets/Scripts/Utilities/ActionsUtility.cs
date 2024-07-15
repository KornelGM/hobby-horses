using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ActionsUtility 
{
    public static AvailableActions ReadActionsBetween([CanBeNull] ServiceLocator caller, [CanBeNull] ServiceLocator interactionItem)
    {
        if (caller == null)
        {
            Debug.LogError("You are trying to read actions while caller object is null");
            return new AvailableActions();
        }

        if (!caller.TryGetServiceLocatorComponent(out InteractionInfo callerInteractionInfo)) return new AvailableActions();

        if (interactionItem == null) return callerInteractionInfo.GetActionsWithSelf();

        interactionItem.TryGetServiceLocatorComponent(out InteractionInfo detectedInteractionInfo);//Don't need to check, we can pass null
        if (!interactionItem.TryGetServiceLocatorComponent(out ItemDataContainer detectedDataContainer)) return callerInteractionInfo.GetActionsWithSelf();

        return ReadActionsBetween(callerInteractionInfo, detectedInteractionInfo, detectedDataContainer.ItemData);
    }

    public static AvailableActions ReadActionsBetween(ServiceLocator caller, [CanBeNull] InteractionInfo info, ItemData data)
    {
        if (!caller.TryGetServiceLocatorComponent(out InteractionInfo callerInteractionInfo))
        {
            Debug.LogError($"You are trying to read actions from {caller}, but it doesn't have {typeof(InteractionInfo)}");
            return new AvailableActions();
        }

        return ReadActionsBetween(callerInteractionInfo, info, data);
    }

    public static AvailableActions ReadActionsBetween(InteractionInfo caller, [CanBeNull] InteractionInfo info, ItemData data)
    {
        if (caller == null)
        {
            Debug.LogError("You are trying to read actions while caller object is null");
            return new AvailableActions();
        }

        if (data == null) return caller.GetActionsWithSelf();

        return caller.GetPossibleActionsWithObject(data, info);
    }

    public static bool TryGetTooltipsFromAvailableActions(Dictionary<InteractionType, IAction> actions, out Dictionary<InteractionType, IActionTooltip> tooltipsOnScreen)
    {
        return TryGetTooltipsFromAvailableActions(actions, out _ , out tooltipsOnScreen, out _);
    }

    public static IActionTooltip GetMainTooltipOnObject(Dictionary<InteractionType, IAction> actions)
    {
        if (!TryGetTooltipsFromAvailableActions(actions, out _, out _, out IActionTooltip mainSction)) return null;
        return mainSction;
    }

	
	public static bool HasAnyActionsWithObject(this ServiceLocator caller, ServiceLocator interactionItem)
    {
        if (caller == null) return false;
        if (interactionItem == null) return false;
        if (!caller.TryGetServiceLocatorComponent(out InteractionInfo callerInteractionInfo)) return false;
        if (!interactionItem.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer)) return false;
        if (itemDataContainer.ItemData == null) return false;
        
		if (callerInteractionInfo.InteractionsAsCaller is not { Count: > 0 }) return false;
		if (callerInteractionInfo.InteractionsAsCaller.Any(container => container.ItemData == itemDataContainer.ItemData)) return true;

		return false;
	}

    public static bool TryGetTooltipsFromAvailableActions(Dictionary<InteractionType, IAction> actions,
        out Dictionary<InteractionType, IActionTooltip> tooltipsOnObject,
        out Dictionary<InteractionType, IActionTooltip> tooltipsOnScreen,
        out IActionTooltip mainTooltip)
    {
        if (actions.Count == 0) 
        {
            tooltipsOnObject = null;
            tooltipsOnScreen = null;
            mainTooltip = null;
            return false;
        }

        Dictionary<InteractionType, IActionTooltip> actionTooltips = actions.Where(action => action.Value != null && action.Value.Tooltip != null).
            GroupBy(action => action.Value.Tooltip).
            Select(group => group.First()).ToDictionary(key => key.Key, value => value.Value.Tooltip);

        tooltipsOnObject = new();
        tooltipsOnScreen = new();

        foreach(var actionTooltip in actionTooltips)
        {
            if (actionTooltip.Value.ShowOnObject) tooltipsOnObject.Add(actionTooltip.Key, actionTooltip.Value);
            else tooltipsOnScreen.Add(actionTooltip.Key, actionTooltip.Value);
        }

        if(tooltipsOnObject.Count == 0)
        {
            mainTooltip = null;
        }
        else
        {
            mainTooltip = tooltipsOnObject.OrderByDescending(action => action.Value.Priority).First().Value;
        }

        return true;
    }
}
