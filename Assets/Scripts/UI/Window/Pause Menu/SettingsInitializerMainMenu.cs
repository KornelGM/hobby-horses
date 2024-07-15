using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsInitializerMainMenu : MonoBehaviour, IServiceLocatorComponent
{
   public MainMenuWindow MainMenuWindow => _mainMenuWindow;

   [SerializeField] private SettingsMenuUI _settingsMenuUI;
   [SerializeField] private MainMenuWindow _mainMenuWindow;
   [ServiceLocatorComponent] private WindowManager _windowManager;
   public ServiceLocator MyServiceLocator { get; set; }

   private void Start()
   {
      _windowManager.RegisterExistingWindow(_mainMenuWindow, true);
      //_windowManager.RegisterExistingWindow(_settingsMenuUI);
      
      _settingsMenuUI.gameObject.SetActive(true);
      foreach (var settingWindow in _settingsMenuUI.MyObject.GetComponentsInChildren<SettingsWindow>(true))
      {
         settingWindow.Initialize();
      }

      _settingsMenuUI.gameObject.SetActive(false);
   }
}
