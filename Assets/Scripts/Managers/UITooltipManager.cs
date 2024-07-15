using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITooltipManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public event Action<UITooltipContent> OnShowTooltip=null;
    public event Action OnHideTooltip=null;
    
    public void ShowTooltip(UITooltipContent content)
    {
        OnShowTooltip?.Invoke(content);
    }

    public void HideTooltip()
    {
        OnHideTooltip?.Invoke();
    }
}