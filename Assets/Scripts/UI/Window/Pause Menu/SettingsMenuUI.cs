using System;
using I2.Loc;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour, IWindow, IServiceLocatorComponent, IUpdateable
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; } = 90;
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldBeCached { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; }

    public bool Enabled { get; } = true;

    [ServiceLocatorComponent] private ModalWindowManager _quitWithoutSavingModalTitle;

    [SerializeField] private Button[] _settingsCategoryButtons;
    [SerializeField] private GameObject[] _settingsCategoryScrollviews;
    [SerializeField] private SettingsWindow _settingsWindow;
    [SerializeField] private GameObject _applyButton;
    [SerializeField] private GameObject _resetToDefaultButton;

    [SerializeField] private LocalizedString _abandonChangesModalTitle;

    private void Start()
    {
        _applyButton.IsNotNull(this, nameof(_applyButton));
        _resetToDefaultButton.IsNotNull(this, nameof(_resetToDefaultButton));
        OpenSettingsPanel(0);
    }

    public void CustomUpdate()
    {
        //if (Input.GetKeyUp(KeyCode.Escape))
        //{
        //    if (!IsOnTop) return;
        //    if (_settingsWindow != null)
        //    {
        //        _settingsWindow.CreateModalForBackButton();
        //    }
        //    else
        //    {
        //        CloseSettingsWindow();
        //    }
        //}
    }

    public void OpenSettingsPanel(int settingsCategoryIndex)
    {
        if (_settingsWindow == null || !_settingsWindow._settingsTab.HasChanged())
        {
            ActivatePanel(settingsCategoryIndex);
            return;
        }

        _quitWithoutSavingModalTitle.CreateModalWindowYesNo(() => { ActivatePanel(settingsCategoryIndex); },
            () => { },
            _abandonChangesModalTitle);
    }

    private void ActivatePanel(int settingsCategoryIndex)
    {
        _settingsCategoryButtons.ForEach(button => button.interactable = true);
        _settingsCategoryScrollviews.ForEach(panel => panel.SetActive(false));

        _settingsCategoryButtons[settingsCategoryIndex].interactable = false;
        _settingsCategoryScrollviews[settingsCategoryIndex].SetActive(true);

        if (_settingsWindow != null)
        {
            _settingsWindow._settingsTab.RevertTabSettingsValues();
        }

        var foundSettingsWindow =
            _settingsCategoryScrollviews[settingsCategoryIndex].GetComponentInChildren<SettingsWindow>();
        _settingsWindow = foundSettingsWindow;
        if (foundSettingsWindow != null)
        {
            _settingsWindow.Initialize();
        }

        _applyButton.SetActive(foundSettingsWindow != null);
        _resetToDefaultButton.SetActive(foundSettingsWindow != null);
    }

    public void TryToQuitWithBackButton()
    {
        if (_settingsWindow != null) _settingsWindow.CreateModalForBackButton();
        else CloseSettingsWindow();
    }

    public void TryToQuitWithBackButtonMainMenu()
    {
        if (_settingsWindow != null) _settingsWindow.CreateModalForBackButton(true);
        else CloseSettingsWindowMainMenu();
    }

    public void CloseSettingsWindow()
    {
        _settingsWindow?._settingsTab?.RevertTabSettingsValues();
        (this as IWindow).DeleteWindow();
    }

    public void CloseSettingsWindowMainMenu()
    {
        _settingsWindow?._settingsTab?.RevertTabSettingsValues();
        gameObject.SetActive(false);
    }

}