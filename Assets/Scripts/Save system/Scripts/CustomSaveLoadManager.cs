using UnityEngine;

public class CustomSaveLoadManager : SaveLoadManager<SaveData, BaseHeaderData>, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private CustomSaveLoadInfo _customSaveLoadInfo;
    protected override SaveLoadInfo<SaveData> _saveLoadInfo => _customSaveLoadInfo;

    protected override ISaveable<BaseHeaderData> GetHeaderData()
    {
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out SceneSaveService sceneSaveService)) return null;
        return sceneSaveService;
    }

    protected override ISaveable<SaveData> GetSaveSequence()
    {
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out SceneSaveService sceneSaveService)) return null;
        return sceneSaveService;
    }
}
