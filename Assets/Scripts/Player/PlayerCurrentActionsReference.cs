using JetBrains.Annotations;
using System;
using System.Collections.Generic;

public class PlayerCurrentActionsReference : IServiceLocatorComponent, IUpdateable, IAvailableActions
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private PlayerManager _playerManager;

    public bool Enabled => AvailableActions.Interactions.Count > 0;
    public AvailableActions AvailableActions { get; private set; } = new AvailableActions();
    public Dictionary<InteractionType, IAction> CurrentActions {get; set;} = new();
    public Action<InteractionType, IAction> OnCurrentActionChanged { get; set; }

    private ServiceLocator _heldItem;
    private ServiceLocator _detectedItem;

    public void SetupActions(ServiceLocator itemInHand, ServiceLocator detectedItem)
    {
        AvailableActions availableActions = ActionsUtility.ReadActionsBetween(itemInHand, detectedItem);
        SetupActions(availableActions, itemInHand, detectedItem);
    }

    public void SetupActions(AvailableActions availableActions, ServiceLocator heldItem, ServiceLocator detectedItem)
    {
        _heldItem = heldItem;
        _detectedItem = detectedItem;
        AvailableActions = availableActions;
        RefreshCurrentActions();
    }

    public void CustomUpdate() => RefreshCurrentActions();

    private void RefreshCurrentActions()
    {
        foreach (var actions in AvailableActions.Interactions)
        {
            RefreshCurrentActions(actions.Key, actions.Value);
        }
    }

    private void RefreshCurrentActions(InteractionType interactionType, List<IAction> actions)
    {
        IAction foundAction = GetAvailableAction(actions);

        if (CurrentActions.ContainsKey(interactionType))
        {
            if (CurrentActions[interactionType] == foundAction) return;
            
            CurrentActions[interactionType] = foundAction;
            OnCurrentActionChanged?.Invoke(interactionType, foundAction);
            return;
        }

        CurrentActions.Add(interactionType, foundAction);
        OnCurrentActionChanged?.Invoke(interactionType, foundAction);
    }

    public void CancelAllActions()
    {
        if (!_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerStateMachine playerStateMachine)) return;
        if (!_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerInputBlocker playerInputBlocker)) return;

        if (_detectedItem == null || _heldItem == null)
            return;

        if(playerStateMachine.CurrentState is PlayerMinigameState || ((IBlocker)playerInputBlocker).IsBlocked)
        {
            foreach (var action in AvailableActions.Interactions)
            {
                if (action.Key == InteractionType.CancelPrimaryInteraction ||
                    action.Key == InteractionType.CancelSecondaryInteraction ||
                    action.Key == InteractionType.CancelAdditiveInteraction ||
                    action.Key == InteractionType.CancelMoreInfo)
                {
                    foreach (var item in action.Value)
                    {
                        if (item.Available(_playerManager.LocalPlayer, _heldItem, _detectedItem))
                            item.Perform(_playerManager.LocalPlayer, _detectedItem, _heldItem);
                    }
                }
            }
        }
    }

    public IAction GetAvailableAction(InteractionType interactionType)
    {
        if (!CurrentActions.ContainsKey(interactionType)) return null;
        return CurrentActions[interactionType];
    }

    [CanBeNull]
    private IAction GetAvailableAction(List<IAction> actions)
    {
        foreach (IAction action in actions)
        {
            if (!action.Available(MyServiceLocator, _heldItem, _detectedItem)) continue;

            return action;
        }
        return null;
    }
}
