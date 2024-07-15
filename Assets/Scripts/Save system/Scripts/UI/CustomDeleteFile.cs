using I2.Loc;
using System;
using UnityEngine;

public class CustomDeleteFile : DeleteFileButton<SaveData, BaseHeaderData>, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private ModalWindowManager _modalWindow;
    [ServiceLocatorComponent] private CustomSaveLoadManager _saveLoadManagerRef;
    [ServiceLocatorComponent] private CustomSaveDataPanelsController _controllerRef;

    [SerializeField] private LocalizedString _areYouSure;
    [SerializeField] private LocalizedString _cantRemove;
    [SerializeField] private LocalizedString _successTitle;
    [SerializeField] private LocalizedString _successDescription;

    public ServiceLocator MyServiceLocator { get; set; }
    protected override SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager => _saveLoadManagerRef;
    protected override SaveDataPanelsController<SaveData, BaseHeaderData> _controller => _controllerRef;

    protected override void ShowModalWindowForDelete(SaveDataPanel<SaveData, BaseHeaderData> panel, Action remove)
    {
        _modalWindow.CreateModalWindowYesNo(remove, null, _areYouSure, $"{panel.SaveDataInfo.FileName}?");
    }

    protected override void ShowModalWindowForDeleteError(SaveDataPanel<SaveData, BaseHeaderData> panel)
    {
        _modalWindow.CreateModalWindowConfirm(null, "", $"{_cantRemove} {panel.SaveDataInfo.FileName}");
    }

    protected override void ShowModalWindowForSuccesfullDelete(SaveDataPanel<SaveData, BaseHeaderData> panel)
    {
        _modalWindow.CreateModalWindowConfirm(null, _successTitle, $"{_successDescription} {panel.SaveDataInfo.FileName}");
    }
}
