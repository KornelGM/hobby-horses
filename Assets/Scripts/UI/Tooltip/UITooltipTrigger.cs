using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private UITooltipManager _tooltipManager = null;
    [SerializeField] private UITooltipContent _tooltipContent = new UITooltipContent();

    private bool _isTriggered = false;

    void OnDisable()
    {
        if (_isTriggered) DisableTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivateTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DisableTooltip();
    }

    public void SetTooltipContent(UITooltipContent newContent)
    {
        _tooltipContent = newContent;
    }

    private void ActivateTooltip()
    {
        if(!_tooltipContent.UseI2)
        {
            if (string.IsNullOrEmpty(_tooltipContent.Title) && string.IsNullOrEmpty(_tooltipContent.Description))
                return;
        }
        else
        {
            if (string.IsNullOrEmpty(_tooltipContent.TitleI2) && string.IsNullOrEmpty(_tooltipContent.DescriptionI2))
                return;
        }

        _isTriggered = true;

        _tooltipManager.ShowTooltip(_tooltipContent);
    }

    private void DisableTooltip()
    {
        _isTriggered = false;

        _tooltipManager.HideTooltip();
    }
}
