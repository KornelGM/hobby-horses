using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

[Serializable]
public abstract class BaseAction : MonoBehaviour, IAction
{
    public event Action OnActionFinished = null;
    public int ActionPriority => Tooltip == null? 0: Tooltip.Priority;
	public IActionTooltip Tooltip { get => _tooltip; set => _tooltip = value as ActionTooltip; }
    public virtual ClampedValue ProgressBar { get; set; } = null;
    public bool IsActionForAI => IsUsableByAI || IsControllableByAI;
    [field: SerializeField, FoldoutGroup("AI Interaction")] public bool IsUsableByAI { get; set; }
    [field: SerializeField, FoldoutGroup("AI Interaction")] public bool IsControllableByAI { get; set; }
    [field: SerializeField, FoldoutGroup("AI Interaction")] public bool HasAdditionalOperations { get; set; }
    
    [SerializeField] public ActionTooltip _tooltip;

    public virtual bool Available(ServiceLocator playerServiceLocator, ServiceLocator itemInHand,
        ServiceLocator detectedItem)
    {
        return true;
    }

    public virtual void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) 
    {
        
    }

    public virtual void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator,
        ServiceLocator itemInHand, float valueToPass)
    {
        
    }
    
    public virtual void OnStart() { }
    public virtual void OnStop() { }

    public virtual void StartFinishOfInteraction() { }

    public void ClearActionFinishedEvent() => OnActionFinished = null;
    public void InvokeActionFinished() => OnActionFinished?.Invoke();
}