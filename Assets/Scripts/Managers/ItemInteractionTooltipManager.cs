using System.Collections.Generic;
using UnityEngine;

public class ItemInteractionTooltipManager : MonoBehaviour, IManager, IAwake, IStartable, IUpdateable, IServiceLocatorComponent
{
    private class ScreenTooltipPoint : ITooltipPoint
    {
        public Vector2 ScreenPos;
        public Vector3 Point => ScreenPos;
        public void UpdatePosition() { }

        public ScreenTooltipPoint(Vector2 screenPos)
        {
            ScreenPos = screenPos;
        }
    }

    public ServiceLocator MyServiceLocator { get; set; }
    public AdditionalInfo AdditionalInfo => _currentAdditionalInfo;
    public bool Enabled => true;

    [SerializeField] private InteractableTooltipPanel _panelPrefab;
    [SerializeField] private InteractableTooltipPanel _sidePanelPrefab;
    [SerializeField] private Vector2 _screenTooltipPosition;

    [ServiceLocatorComponent] private WindowManager _windowManager;

    private InteractableTooltipPanel _sidePanel;
    private ScreenTooltipPoint _screenTooltipPoint;
    private Camera _camera;
    private InteractableTooltipPanel _currentCentralPanel;
    private AdditionalInfo _currentAdditionalInfo;
    private ITooltipPoint _currentTooltipPoint;

    private List<object> _hiders = new();

    private bool _hidden => _hiders.Count != 0;

    public void CustomAwake()
    {
        _screenTooltipPoint = new(_screenTooltipPosition);
        _sidePanel = _windowManager.CreateWindow(_sidePanelPrefab).GetComponent<InteractableTooltipPanel>();
        _sidePanel.CustomAwake();
        _sidePanel.HidePanel();
    }

    public void CustomStart()
    {
        _panelPrefab.IsNotNull(this, nameof(_panelPrefab));
        _camera = Camera.main;
    }

    public void CustomUpdate() => UpdatePanelsPosition();

    public void CustomReset()
    {

    }

    public void ToggleSidePanel(bool toggle)
    {
        _sidePanel.transform.localScale = toggle ? Vector3.one : Vector3.zero;
    }

    public void ShowTooltipOnScreen(IItemInteractionTooltip tooltipInfo) => ShowTooltip(_screenTooltipPoint, tooltipInfo);

    /// <summary>
    /// Use this to show some tooltip. There is only one panel for each ITooltipPoint
    /// </summary>
    /// <param name="tooltipPoint">Tooltip target. Null means that panel will be shown on screen</param>
    /// <param name="tooltipInfo">Info to show on panel</param>
    public void ShowTooltip(ITooltipPoint tooltipPoint, IItemInteractionTooltip tooltipInfo)
    {
        if (tooltipInfo == null) return;

        InteractableTooltipPanel spawnedPanel = GetPanel(tooltipPoint);

        if (spawnedPanel == null)
        {
            Debug.LogError("Spawned panel for some reason is null");
            return;
        }

        spawnedPanel.HidePanel();
        spawnedPanel.ShowPanel(tooltipInfo);
        UpdatePanel(tooltipPoint);
    }

    public void HideScreenAction(InteractionType type) => RefreshScreenAction(type, null);
    public void RefreshScreenAction(InteractionType type, IActionTooltip tooltip) => RefreshAction(_screenTooltipPoint, type, tooltip);
    public void HideAction(ITooltipPoint tooltipPoint, InteractionType type) => RefreshAction(tooltipPoint, type, null);
    public void RefreshAction(ITooltipPoint tooltipPoint, InteractionType type, IActionTooltip tooltip)
    {
        InteractableTooltipPanel panel = GetPanel(tooltipPoint);
        if (panel == null) return;

        panel.ShowAction(type, tooltip);
    }

    private InteractableTooltipPanel GetPanel(ITooltipPoint tooltipPoint)
    {
        if (tooltipPoint == null) return null;

        if (tooltipPoint == _screenTooltipPoint) return _sidePanel;

        _currentTooltipPoint = tooltipPoint;
        if (_currentCentralPanel != null) return _currentCentralPanel;

        _currentCentralPanel = _windowManager.CreateWindow(_panelPrefab).GetComponent<InteractableTooltipPanel>();
        _currentAdditionalInfo = _currentCentralPanel.GetAdditionalInfo();

        return _currentCentralPanel;
    }

    public void HideTooltipOnScreen() => _sidePanel?.HideAllActions();
    public void HideTooltip(ITooltipPoint tooltipPoint)
    {
        if (tooltipPoint == null) return;
        _currentCentralPanel?.HidePanel();
    }

    private void UpdatePanelsPosition()
    {
        if (_currentTooltipPoint != null)
            UpdatePanel(_currentTooltipPoint);
    }

    private void UpdatePanel(ITooltipPoint tooltipPoint)
    {
        TooltipPoint point = tooltipPoint as TooltipPoint;
        ScreenTooltipPoint screenPoint = tooltipPoint as ScreenTooltipPoint;

        if (point == null && screenPoint == null)
            tooltipPoint = _screenTooltipPoint;

        if (tooltipPoint == _screenTooltipPoint)
        {
            _sidePanel.SetAnchoredPosition(_screenTooltipPosition);
            return;
        }

        InteractableTooltipPanel panel;

        if (_currentCentralPanel == null)
            panel = GetPanel(tooltipPoint);
        else
            panel = _currentCentralPanel;

        tooltipPoint.UpdatePosition();
        panel.transform.position = _camera.WorldToScreenPoint(tooltipPoint.Point);
    }

    public void Hide(object obj)
    {
        if (!_hidden) Hide();

        _hiders.Add(obj);
    }

    public void Unhide(object obj)
    {
        _hiders.Remove(obj);

        if (!_hidden) Unhide();
    }

    private void Hide()
    {
        if(_currentCentralPanel != null)
            _currentCentralPanel.gameObject.SetActive(false);
    }

    private void Unhide()
    {
        if (_currentCentralPanel != null)
            _currentCentralPanel.gameObject.SetActive(true);
    }
}