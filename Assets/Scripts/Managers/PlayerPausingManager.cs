using System;
using UnityEngine;

public class PlayerPausingManager : MonoBehaviour, IServiceLocatorComponent, IManager, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    public event Action<bool> OnPause;

    [SerializeField] private PauseMenuUI pauseMenuUIPrefab;

    [ServiceLocatorComponent] private HobbyHorsePlayerManager _playerManager;
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent] private TimePausingManager _timePausingManager;

    private PlayerInputReader _playerInputReader;
    private PlayerInputBlocker _playerInputBlocker;
    private PauseMenuUI _createdPauseMenuUI;
    private IVirtualController _virtualController;
    private HobbyHorseStateMachine _stateMachine;

    public void CustomStart()
    {
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _stateMachine);

        if (_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _virtualController))
        {
            _virtualController.OnPausePerformed += TryPauseGame;
            _virtualController.OnUnpausePerformed += TryPauseGame;
        }
    }

    private void TryPauseGame()
    {
        if (_windowManager.TopPriorityWindow() == null || _windowManager.TopPriorityWindow().Priority == 0)
        {
            PauseGame();
        }
        else
        {
            _windowManager.TryToCloseTopWindow();
        }
    }

    private void Update()
    {
        //Reading input for testing purposes only
        //if (_stateMachine != null)
        //{
        //    PlayerInputGameplayState playerInputGameplayState =
        //        _stateMachine.PlayerController.PlayerInputStateMachine.CurrentState as PlayerInputGameplayState;

        //    if (playerInputGameplayState == null) return;
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    if (_playerInputBlocker == null)
        //    {
        //        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        //    }

        //    if (_playerInputReader == null)
        //    {
        //        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputReader);
        //    }

        //    InputBlockerSettings blockerSettings = new InputBlockerSettings(this, inputPauseState, OpenWindow);

        //    _playerInputBlocker.Block(blockerSettings);
        //}

        //void OpenWindow()
        //{
        //    _modalWindowManager.CreateModalWindowConfirm(OnCloseModal,
        //        "Test Modal Window",
        //        "Modal window's description. Confirm to continue");
        //}
    }

    public void UnpauseGame()
    {
        if (_playerInputBlocker == null) return;

        _windowManager.SetMainCanvasOrder();

        _playerInputBlocker.TryUnblock(this);
    }

    private void PauseGame()
    {
        if (_playerInputBlocker == null)
        {
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        }

        if (_playerInputReader == null)
        {
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputReader);
        }

        if (((IBlocker)_playerInputBlocker).IsBlocked)
        {
            return;
        }

        _windowManager.SetMainCanvasOrder(110);

        OnPause?.Invoke(true);

        InputBlockerSettings blockerSettings = new InputBlockerSettings(this,
            new PlayerInputPauseState(_playerInputBlocker.InputManager),
            PauseBegin,
            PauseEnd);

        _playerInputBlocker.Block(blockerSettings);
    }

    private void PauseBegin()
    {
        _createdPauseMenuUI = _windowManager.CreateWindow(pauseMenuUIPrefab, WindowManager.PauseMenuBasePriority)
            .GetComponent<PauseMenuUI>();
        _createdPauseMenuUI.PausingManager = this;

        _stateMachine.SwitchToPasueState();

        _timePausingManager.Pause();
    }

    private void PauseEnd()
    {
        OnPause?.Invoke(false);
        _windowManager.DeleteWindow(_createdPauseMenuUI);

        _stateMachine.SwitchToMoveState();

        _timePausingManager.Unpause();
    }

    private void OnCloseModal()
    {
        _playerInputBlocker.TryUnblock(this);
    }

}