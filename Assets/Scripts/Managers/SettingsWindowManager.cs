using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsWindowManager : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    public SettingsMenuUI CreatedSettingsMenuUI => _createdSettingsMenuUI;


    [ServiceLocatorComponent] private WindowManager _windowManager;

    [SerializeField] private SettingsMenuUI _settingsMenuUIPrefab;

    private SettingsMenuUI _createdSettingsMenuUI;

    public void CustomStart()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        //_createdSettingsMenuUI = _windowManager.CreateWindow(_settingsMenuUIPrefab, 
        //    WindowManager.PopUpBasePriority).GetComponent<SettingsMenuUI>();

        foreach (var settingWindow in _settingsMenuUIPrefab.GetComponentsInChildren<SettingsWindow>(true))
        {
            settingWindow.Initialize();
        }

        //_createdSettingsMenuUI.gameObject.SetActive(false);
    }
}
