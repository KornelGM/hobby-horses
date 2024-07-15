using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnClickUI : MonoBehaviour, IServiceLocatorComponent, IPointerDownHandler,IPointerUpHandler
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CursorManager _cursorManager;
    private ButtonBlocker _buttonBlocker;
    private bool _isHolding;

    private void Start()
    {
        _buttonBlocker = GetComponent<ButtonBlocker>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isHolding) return;
        
        if (_buttonBlocker == null || _buttonBlocker.IsCurrentlyBlocked == false) _cursorManager.StartClick();
        _isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cursorManager.EndClick();
        _isHolding = false;
    }
}