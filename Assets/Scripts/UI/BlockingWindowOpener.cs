using UnityEngine;
using UnityEngine.Events;

public class BlockingWindowOpener<T> 
    where T: IWindow
{
    public ServiceLocator MyServiceLocator;

    private WindowManager _windowManager;
    private PlayerManager _playerManager;
    private QuestManager _questManager;
    private ItemInteractionTooltipManager _tooltipManager;

    private T _windowToOpen;
    private T _createdWindow;
    private PlayerInputReader _playerInputReader;
    private PlayerInputBlocker _playerInputBlocker;
    private InteractableSelector _playerInteractableSelector;
    private DreamParableLogger.Logger _logger;

    public UnityAction OnStart;
    public UnityAction OnEnd;

    private bool _initialized = false;

    private void Initialize()
    {
        MyServiceLocator = SceneServiceLocator.Instance;
        if (MyServiceLocator == null) return;
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out _windowManager)) return;
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out _playerManager)) return;
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out _questManager)) return;
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out _tooltipManager)) return;
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out _logger)) return;

        _initialized = true;
    }

    public void OnOpenWindow(T windowToOpen, out T window, State inputState = null)
    {
        if (!_initialized)
            Initialize();

        window = default;
        _windowToOpen = windowToOpen; 

        if (_playerInputBlocker == null)
        {
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        }

        if (_playerInputReader == null)
        {
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputReader);
        }

        if (_playerInteractableSelector == null)
        {
            _playerInputBlocker.MyServiceLocator.TryGetServiceLocatorComponent(out _playerInteractableSelector);
        }

        if (_playerInteractableSelector != null)
        {
            _playerInteractableSelector.Block(this);
        }

        if (((IBlocker)_playerInputBlocker).IsBlocked)
        {
            
        }

        if (inputState == null)
            inputState = new PlayerInputBookState(_playerInputBlocker.InputManager);


        InputBlockerSettings blockerSettings = new InputBlockerSettings(this, inputState,
            PauseBegin,
            PauseEnd);

        _playerInputBlocker.Block(blockerSettings);

        window = CreateWindow();
    }

    protected void PauseBegin()
    {
        OnStart?.Invoke();
    }

    protected virtual T CreateWindow()
    {
        _createdWindow = _windowManager.CreateWindow(_windowToOpen, 
            WindowManager.PauseMenuBasePriority).GetComponent<T>();

        _questManager.ToggleQuestPanel(false);

        if (_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerQuickAccessItemsBar itemsBar))
        {
            itemsBar.ToggleItemSlotVisualiser(false);
            itemsBar.ToggleItemOnCurrentSlot(false);
        }

        _tooltipManager.ToggleSidePanel(false);

        _logger.Log(LogType.Log, "Blocking Window Opener", this, $"Created {_createdWindow.MyObject.name}");
        return _createdWindow;
    }

    protected virtual void DeleteWindow()
    {
        if (_createdWindow == null) return;
        _logger.Log(LogType.Log, "Blocking Window Opener", this, $"Delete {_createdWindow.MyObject.name}");
        _windowManager.DeleteWindow(_createdWindow);
    }

    public void UnblockPlayer()
    {
        if (_playerInteractableSelector != null)
        {
            _playerInteractableSelector.Unblock(this);
        }

        _questManager.ToggleQuestPanel(true);

        if (_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerQuickAccessItemsBar itemsBar))
        {
            itemsBar.ToggleItemSlotVisualiser(true);
            itemsBar.ToggleItemOnCurrentSlot(true);
        }

        _tooltipManager.ToggleSidePanel(true);

        _playerInputBlocker.TryUnblock(this);
    }

    protected void PauseEnd()
    {
        OnEnd?.Invoke();
        DeleteWindow();
    }
}
