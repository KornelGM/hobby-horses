using I2.Loc;
using System;
using UnityEngine;

public class CustomLoadWindow : LoadWindow<SaveData, BaseHeaderData>, IWindow, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private CustomSaveLoadManager _cSaveLoadManager;
    [ServiceLocatorComponent] private CustomSaveDataPanelsController _cPanelsController;
    [ServiceLocatorComponent] private CustomSceneLoader _cSceneLoader;
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent] private LoadingWindow _sceneLoader;

    [SerializeField] private LocalizedString _selectFile;
    [SerializeField] private LocalizedString _areYouSure;
    [SerializeField] private LocalizedString _progressWontBeSaved;

    public ServiceLocator MyServiceLocator { get; set; }

    public WindowManager Manager { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; } = true;
    public GameObject MyObject { get => gameObject; }
    public int Priority { get; set; } = 100;
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldBeCached { get; set; } = true;

    protected override SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager => _cSaveLoadManager;
    protected override SaveDataPanelsController<SaveData, BaseHeaderData> _controller => _cPanelsController;

    public override void DeleteWindow() { Manager.DeleteWindow(this); }

    protected override string GetSceneName(SaveData saveData)
    {
        if (saveData == null) return "";
        return saveData.SceneName;
    }

    protected override void TryLoadSelected(SaveDataPanel<SaveData, BaseHeaderData> panel)
    {
        base.TryLoadSelected(panel);
        _sceneLoader.OnSceneLoad(GetSceneName(_saveLoadInfo.CurrentData));
    }

    protected override void ShowModalForBeSureToLoad(SaveDataPanel<SaveData, BaseHeaderData> panel, Action action)
    {
        _modalWindowManager.CreateModalWindowYesNo(action, null, _areYouSure, _progressWontBeSaved);
    }

    protected override void ShowModalForUnSelectingFile()
    {
        _modalWindowManager.CreateModalWindowConfirm(null, _selectFile);
    }
}
