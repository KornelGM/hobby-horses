using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableTooltipPanel : MonoBehaviour, IServiceLocatorComponent, IAwake, IWindow
{
    [SerializeField] private InteractionType _interactiontype;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private ClampedValueIcon _progressIcon;
    [SerializeField] private AdditionalInfo _additionalInfo;

    [SerializeField] private List<ActionTooltipPanel> _actionPanels = new List<ActionTooltipPanel>();

    public ServiceLocator MyServiceLocator { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = false;
    public WindowManager Manager { get; set; }

    private RectTransform _rectTransform;

    public void CustomAwake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public AdditionalInfo GetAdditionalInfo()
    {
        return _additionalInfo;
    }

    public void ShowPanel(IItemInteractionTooltip tooltip)
    {
        if (tooltip == null)
        {
            Debug.LogError("You are trying to show null tooltip");
        }

        gameObject.SetActive(true);
        if(_name)_name.text = tooltip.Name;
        if (_progressIcon != null) _progressIcon.Initialize(tooltip.ProgressBar);

        if (tooltip.Actions == null) return;

        foreach (var actionTooltip in tooltip.Actions)
        {
            ShowAction(actionTooltip.Key, actionTooltip.Value);
        }
    }

    public void ShowAction(InteractionType interaction, IActionTooltip tooltip)
    {
        if (tooltip == null)
        {
            HideAction(interaction);
            return;
        }

        ActionTooltipPanel panel = GetPanel(interaction);
        if(panel != null) panel.ShowAction(tooltip);
    }

    public void HideAllActions()
    {
        _actionPanels.ForEach(action => action.Hide());
    }

    public void HideAction(InteractionType interaction)
    {
        GetPanel(interaction).Hide();
    }

    public void SetAnchoredPosition(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }

    public void HidePanel()
    {
        _additionalInfo?.SetOnHideToolTipsEvent(OnHideTooltips);

        if (_name) _name.text = "";

        HideAllActions();

        void OnHideTooltips()
        {
           // gameObject.SetActive(false);
        }

    }

    public void OnDeleteWindow()
    {

    }

    public void DeleteWindow()
    {
        Manager.DeleteWindow(this);
    }

    private ActionTooltipPanel GetPanel(InteractionType type)
    {
        foreach (ActionTooltipPanel panel in _actionPanels)
        {
            if (panel.CompareType(type)) return panel;
        }

        Debug.LogError($"There is no panel for {type}");
        return null;
    }

    public bool ShouldDeactivateCrosshair { get; set; }
}
