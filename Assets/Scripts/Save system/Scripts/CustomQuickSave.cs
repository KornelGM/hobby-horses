using I2.Loc;
using System;
using System.Collections;
using UnityEngine;

public class CustomQuickSave : QuickSave<SaveData, BaseHeaderData>, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent] private CustomSaveLoadManager _saveLoadManagerRef;
    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;
    protected override SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager => _saveLoadManagerRef;

    private PlayerStateMachine _stateMachine;
    private InteractableSelector _interactableSelector;
    private PlayerInputBlocker _playerInputBlocker;

    [SerializeField] private LocalizedString _quickSave;
    [SerializeField] private LocalizedString _quickSaveNotyfication;
    [SerializeField] private LocalizedString _couldntSave;
    [SerializeField] private LocalizedString _couldntSaveDescription;

    protected override string _autoSaveName => _quickSave;
    protected override Action _sendNotificationAction => () => SednNotification();

    private Coroutine _trySaveCoroutine;

    public void CustomStart()
    {
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _stateMachine);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _interactableSelector);

        if(_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out PlayerInputReader inputReader))
        {
            inputReader.OnQuickSavePerformed += StartTryingSave;
        }
    }

    private void StartTryingSave()
    {
        if(_trySaveCoroutine != null)
            StopCoroutine(_trySaveCoroutine);

        _trySaveCoroutine = StartCoroutine(TrySaveCoroutine());
    }

    private IEnumerator TrySaveCoroutine()
    {
        while (_stateMachine.CurrentState is not PlayerMoveState || ((IBlocker)_playerInputBlocker).IsBlocked
            || _interactableSelector.Blocked)
        {
            yield return null;
        }

        _logger.Log(LogType.Log, "Custom Quick Save", this, "Quick saving...");
        TrySave();
    }


    protected override void SaveErrorFeedback()
    {
        _modalWindowManager.CreateModalWindowConfirm(null, _couldntSave, _couldntSaveDescription);
    }
    private void SednNotification()
    {
        if (_notificationsSystem == null)
            return;

        _notificationsSystem.SendSideNotification(_quickSaveNotyfication, NotificationType.Information);
    }
}
