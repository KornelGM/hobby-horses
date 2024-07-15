using System;
using System.Collections.Generic;

public interface IAvailableActions
{
    public AvailableActions AvailableActions { get; }
    public Action<InteractionType, IAction> OnCurrentActionChanged { get; set; }
    public Dictionary<InteractionType, IAction> CurrentActions { get; set; }

    public void SetupActions(AvailableActions actions, ServiceLocator itemInHand, ServiceLocator detectedItem) 
    {
        AvailableActions availableActions = ActionsUtility.ReadActionsBetween(itemInHand, detectedItem);
        SetupActions(availableActions, itemInHand, detectedItem);
    }

    public void SetupActions(ServiceLocator itemInHand, ServiceLocator detectedItem);
    public IAction GetAvailableAction(InteractionType type);
}
