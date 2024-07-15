public class CustomSaveDataPanelsController : SaveDataPanelsController<SaveData, BaseHeaderData>, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private CustomSaveLoadManager _cSaveLoadManager;
    protected override SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager => _cSaveLoadManager;
}
