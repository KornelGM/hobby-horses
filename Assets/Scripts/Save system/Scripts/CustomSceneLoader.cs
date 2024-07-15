using UnityEngine;

public class CustomSceneLoader : SceneLoader<SaveData>, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private CustomSaveLoadInfo _customSaveLoadInfo;
    protected override SaveLoadInfo<SaveData> _saveLoadInfo => _customSaveLoadInfo;
}
