using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class PopUpManager : MonoBehaviour, IServiceLocatorComponent, IManager, IUpdateable, IStartable, ISaveable<SaveData>
{
    public bool Enabled { get; } = true;
    public ServiceLocator MyServiceLocator { get; set; }
    public PopUp[] PopUps => _popUps;

    [SerializeField] private PopUp[] _popUps;
    //[SerializeField] private PopUp testPopup;

    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private PlayerManager _playerManager;

    private PlayerStateMachine _stateMachine;
    private InteractableSelector _interactableSelector;
    private PlayerInputBlocker _playerInputBlocker;
    private BlockingWindowOpener<PopUp> _popupWindowOpenner = new();

    private List<PopupType> _unlockedPopups = new();

    private State inputPauseState => new PlayerInputPauseState(_playerInputBlocker.InputManager);
    public void CustomStart()
    { 
        CustomReset();
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _stateMachine);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _interactableSelector);
    }

    public void CustomUpdate()
    {       
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    AddPopUpToBlockingQueue(testPopup, true);
        //}
    }

    public void AddPopUpToBlockingQueue(PopupType popupType, bool force = false, bool isRelated = false)
    {
        PopUp popUp = _popUps.FirstOrDefault(x => x.PopupType == popupType);

        if(popUp != null && popUp.PopupType != PopupType.None)
        {
            AddPopUpToBlockingQueue(popUp, force, isRelated);
        }
    }

    public void AddPopUpToBlockingQueue(PopUp popUp, bool force = false, bool isRelated = false)
    {
        if(!force)
        {
            if (CheckPopupIsUnlocked(popUp) && !popUp.CanBeShowMoreTime)
                return;
        }

        if(_playerInputBlocker == null)
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        
        popUp.AlreadyUsed = true;

        if(!CheckPopupIsUnlocked(popUp))
            _unlockedPopups.Add(popUp.PopupType);

        StartCoroutine(ShowPopup(popUp.ShowDelay, () => DisplayPopUp(popUp, isRelated)));
    }

    private IEnumerator ShowPopup(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);

        while (_stateMachine.CurrentState is not PlayerMoveState || ((IBlocker)_playerInputBlocker).IsBlocked 
            || _interactableSelector.Blocked)
        {
            yield return null;
        }
        action.Invoke();
    }

    private void DisplayPopUp(PopUp popUp, bool isRelated = false)
    {
        _popupWindowOpenner.OnOpenWindow(popUp, out PopUp createdPopup, inputPauseState);

         createdPopup.Initialize(this,_popupWindowOpenner.UnblockPlayer , _playerManager.LocalPlayer, isRelated);
    }

    [CanBeNull]
    private PopUp GetFirstNeverDisplayedPopUp()
    {
        return _popUps
            .FirstOrDefault(popUp => !popUp.AlreadyUsed);
    }

    public void CustomReset()
    {
        _popUps.ForEach(popUp => popUp.AlreadyUsed = false);
    }

    public bool CheckPopupIsUnlocked(PopUp popup)
    {
        return _unlockedPopups.Contains(popup.PopupType);
    }

    public SaveData CollectData(SaveData data)
    {
        data.UnlockedPopups = new(_unlockedPopups);
        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null)
            return;

        _unlockedPopups = new(save.UnlockedPopups);
    }
}