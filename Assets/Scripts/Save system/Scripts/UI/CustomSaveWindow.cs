using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSaveWindow : SaveWindow<SaveData, BaseHeaderData>, IServiceLocatorComponent, IWindow
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private CustomSaveLoadManager _cSaveLoadManager;
    [ServiceLocatorComponent] private CustomSaveDataPanelsController _cPanelsController;
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;

    [SerializeField] private LocalizedString _areYouSure;
    [SerializeField] private LocalizedString _nameAlreadyExists;
    [SerializeField] private LocalizedString _overwriteSave;

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

    protected override void OnAlreadyExists(Action action)
    {
        _modalWindowManager.CreateModalWindowYesNo(action, null, _areYouSure, _overwriteSave);
    }

    public void DeleteWindow() { Manager.DeleteWindow(this); }

    protected override void OnAlreadyNameExists(Action action)
    {
        _modalWindowManager.CreateModalWindowYesNo(action, null, _areYouSure, _nameAlreadyExists);
    }

    protected override void ShowModalForSaveError()
    {
        throw new NotImplementedException();
    }
}
