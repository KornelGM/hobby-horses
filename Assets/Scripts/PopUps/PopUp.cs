using I2.Loc;
using Sirenix.OdinInspector;
using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class PopUp : PageableWindowBase, IWindow, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private PopUpManager _popupManager;

    public WindowManager Manager { get; set; }
    public  GameObject MyObject => gameObject;
    public GameObject[] Pages => pages;
    public  int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = true;
    public  bool IsOnTop { get; set; }
    public  bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = true;

    public ServiceLocator MyServiceLocator { get; set; }


    [field: SerializeField] public float ShowDelay { get; set; } = 0f;
    [field: SerializeField] public bool AlreadyUsed { get; set; } = false;
    [field: SerializeField, Header("Encyclopedia Settings")]
    public bool ShouldBeInEncyclopedia { get; set; } = true;

    [field: SerializeField, ShowIf(nameof(ShouldBeInEncyclopedia))]
    public LocalizedString EncyclopediaEntryTitle { get; set; } = String.Empty;
    public event Action<LocalizedString> OnUnlock;

    protected UnityAction _onUnblock;
    protected PopUpManager _popUpManager;

    [Header("Popup info")]
    public PopupType PopupType;
    [field: SerializeField] public bool CanBeShowMoreTime { get; private set; }
    [field: SerializeField] public EncyclopediaPopupPagesController EncyclopediaPopupPagesController { get; private set; }

    [Header("Related Popup info")]
    [SerializeField] private bool _forceRelated;
    [SerializeField] private PopupType _relatedPopup;

    private bool _isRelated;

    public virtual void Initialize(PopUpManager popUpManager, UnityAction unblock, PlayerServiceLocator player, bool isRelated = false)
    {
        _onUnblock = unblock;
        _popUpManager = popUpManager;
        _isRelated = isRelated;
    }
    
    public virtual void OnDeleteWindow()
    {
        _onUnblock?.Invoke();
        if (ShouldBeInEncyclopedia && EncyclopediaEntryTitle != null)
            OnUnlock?.Invoke(EncyclopediaEntryTitle);

        if (_relatedPopup != PopupType.None && !_isRelated)
            _popUpManager.AddPopUpToBlockingQueue(_relatedPopup, _forceRelated, true);
    }

    public virtual void OnExitButton()
    {
        Manager.DeleteWindow(this);
    }
}