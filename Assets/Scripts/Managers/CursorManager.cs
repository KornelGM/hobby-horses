using UnityEngine;

public class CursorManager : MonoBehaviour, IManager, ICursorManager, IStartable, IServiceLocatorComponent, IUpdateable
{
    public bool IsCursorActive { get; private set; } = false;
    public bool Enabled { get; } = true;

    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private Texture2D _defaultCursorTexture;
    [SerializeField] private Texture2D _hoverCursorTexture;
    [SerializeField] private Texture2D _hoverBlockedCursorTexture;
    [SerializeField] private Texture2D _clickedCursorTexture;

    [SerializeField] private bool _setActiveOnStart = false;
    [SerializeField] private float _cursorClickedStateDuration = 0.25f;

    private CursorState _cursorState = CursorState.Default;
    private IVirtualController _virtualController;

    private PlayerManager _playerManager;
    private bool _isHovering;
    private bool _isHoveringBlockedElement;
    private float _timeSinceLastClick = 1f;
    private bool _isCursorDirty;
    private bool _isHoldingClick;

    public void CustomUpdate()
    {
        if (!IsCursorActive) return;
        
        HandleCursorState();
    }

    public void ActivateCursor()
    {
        if (IsCursorActive)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsCursorActive = true;
        _timeSinceLastClick = _cursorClickedStateDuration + 1;

        if (_virtualController != null)
            _virtualController.OnFirstInteractionPerformed += StartParticle;
    }


    public void CollectData(SaveData data)
    {
    }

    public void DeactivateCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsCursorActive = false;

        if(_virtualController != null)
            _virtualController.OnFirstInteractionPerformed -= StartParticle;
    }

    public void EnterHover(bool isElementBlocked = false)
    {
        _isHovering = true;
        _isHoveringBlockedElement = isElementBlocked;
        _isCursorDirty = true;
    }

    public void ExitHover()
    {
        _isHovering = false;
        _isCursorDirty = true;
    }

    public void StartClick()
    {
        _timeSinceLastClick = 0;
        _isHoldingClick = true;
        _isCursorDirty = true;
    }

    public void EndClick()
    {
        _isHoldingClick = false;
    }
    public void CustomReset()
    {
        ActivateCursor();
    }

    public void CustomStart()
    {
        if(SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _playerManager, true))
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _virtualController);

        ChangeCursorState(CursorState.Default);

        if (_setActiveOnStart) ActivateCursor();
        else DeactivateCursor();
    }

    private void HandleCursorState()
    {
        switch (_cursorState)
        {
            case CursorState.Default:
            {
                CursorDefaultState();
                break;
            }
            case CursorState.Hovering:
            {
                CursorHoveringState();
                break;
            }
            case CursorState.Clicked:
            {
                CursorClickedState();
                break;
            }
        }
    }

    private void CursorDefaultState()
    {
        if (_timeSinceLastClick <= _cursorClickedStateDuration)
        {
            ChangeCursorState(CursorState.Clicked);
            return;
        }
        if (_isHovering)
        {
            ChangeCursorState(CursorState.Hovering);
            return;
        }
        if (_isCursorDirty)
        {
            Cursor.SetCursor(_defaultCursorTexture, Vector2.zero, CursorMode.Auto);
            _isCursorDirty = false;
        }
    }

    private void CursorHoveringState()
    {
        if (_timeSinceLastClick <= _cursorClickedStateDuration)
        {
            ChangeCursorState(CursorState.Clicked);
            return;
        }
        if (!_isHovering)
        {
            ChangeCursorState(CursorState.Default);
            return;
        }
        if (_isCursorDirty)
        {
            Cursor.SetCursor(_isHoveringBlockedElement ? _hoverBlockedCursorTexture : _hoverCursorTexture,
                Vector2.zero,
                CursorMode.Auto);
            _isCursorDirty = false;
        }
    }

    private void CursorClickedState()
    {
        _timeSinceLastClick += Time.unscaledDeltaTime;
        if (_timeSinceLastClick > _cursorClickedStateDuration && !_isHoldingClick)
        {
            ChangeCursorState(CursorState.Default);
        }

        if (_isCursorDirty)
        {
            Cursor.SetCursor(_clickedCursorTexture,Vector2.zero, CursorMode.Auto); 
        }
    }

    private void StartParticle()
    {
        if (_cursorState != CursorState.Default)
            return;
    }

    private void ChangeCursorState(CursorState cursorState)
    {
        _cursorState = cursorState;
        _isCursorDirty = true;
    }

    private enum CursorState
    {
        Default,
        Hovering,
        Clicked,
    }
}