using I2.Loc;
using System;
using System.Collections;
using UnityEngine;

public class CustomAutoSave : AutoSave<SaveData, BaseHeaderData>, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;
    [ServiceLocatorComponent] private CustomSaveLoadManager _saveLoadManagerRef;
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;

    protected override string _autoSaveName => _autoSave;
    protected override Action _sendNotificationAction => () => SednNotification();
    protected override SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager => _saveLoadManagerRef;

    [SerializeField] private LocalizedString _autoSave;
    [SerializeField] private LocalizedString _autoSaveNotification;

    private PlayerStateMachine _stateMachine;
    private InteractableSelector _interactableSelector;
    private PlayerInputBlocker _playerInputBlocker;

    public override void CustomStart()
    {
        base.CustomStart();

        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _stateMachine);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _interactableSelector);
    }

    protected override void SaveErrorFeedback() { }

    protected override IEnumerator LoopingAutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(_secondsBetweenSaves);

            while (_stateMachine.CurrentState is not PlayerMoveState || ((IBlocker)_playerInputBlocker).IsBlocked
                || _interactableSelector.Blocked)
            {
                yield return null;
            }

            _logger.Log(LogType.Log, "Custom Auto Save", this, "Auto saving...");
            TrySave();
        }
    }

    private void SednNotification()
    {
        if (_notificationsSystem == null)
            return;

        _notificationsSystem.SendSideNotification(_autoSaveNotification, NotificationType.Information);
    }
}
