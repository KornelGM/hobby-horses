
using UnityEngine;

public class PauseMenuUI : MonoBehaviour, IWindow, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = false;
    public PlayerPausingManager PausingManager { get; set; }
    
    [SerializeField] private SettingsMenuUI _settingsWindow;
    [SerializeField] private CustomLoadWindow _loadWindow;
    [SerializeField] private CustomSaveWindow _saveWindow;

    private bool _alreadyUnpaused;

    public void Resume()
    {
        if (_alreadyUnpaused) return;
        if (!IsOnTop) return;
        _alreadyUnpaused = true;

        PausingManager.UnpauseGame();
    }

    public void OpenSettingsMenu()
    {
        if (!IsOnTop) return;
        Manager.CreateWindow(_settingsWindow,Priority+1);
    }

    public void OpenSaveMenu()
    {
        if (!IsOnTop) return;
        Manager.CreateWindow(_saveWindow, Priority + 1);
    }

    public void OpenLoadMenu()
    {
        if (!IsOnTop) return;
        Manager.CreateWindow(_loadWindow, Priority + 1);
    }

    public void DeleteWindow()
    {
        Manager.DeleteWindow(this);
    }

    public void OnDeleteWindow()
    {
        if (_alreadyUnpaused) return;
        _alreadyUnpaused = true;

        PausingManager.UnpauseGame();
    }
}