using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnHoverUI : MonoBehaviour, IHoverUI, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CursorManager _cursorManager;
    
    private ButtonBlocker _buttonBlocker;
    public void OnPointerEnter(PointerEventData eventData) => PerformActionOnPointerEnter();

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (!eventData.fullyExited) return;

        PerformActionOnPointerExit();
    }

    public void PerformActionOnPointerEnter()
    {
        if (_cursorManager == null) return;
        
        _cursorManager.EnterHover(_buttonBlocker != null && _buttonBlocker.IsCurrentlyBlocked);
    }

    public void PerformActionOnPointerExit()
    {
        if (_cursorManager == null) return;

        _cursorManager.ExitHover();
    }

    private void OnDisable()
    {
        PerformActionOnPointerExit();
    }

    private void Start()
    {
        _buttonBlocker = GetComponent<ButtonBlocker>();
    }
}