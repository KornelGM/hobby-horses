using UnityEngine;

public class SaveLoadMenu : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
	[SerializeField] private string _saveFolder = "TEST";
	[SerializeField] private string _mainMenuSceneName = "Main Menu";
	[SerializeField] private string _mainGameplayScene = "Chocolate Manufacture";
    [SerializeField] private CustomSaveLoadInfo _saveLoadInfo = null;

	[ServiceLocatorComponent] private SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager;
	[ServiceLocatorComponent] private SceneLoader<SaveData> _sceneLoader;

	//Used by button
	public void LoadNewGame()
    {
		_saveLoadManager.ChangeCurrentSave(_saveLoadInfo.GetSaveDataForScene(_mainGameplayScene));
		_sceneLoader.TryLoadScene(_mainGameplayScene);
	}

	//Used by button
	public void LoadGame()
	{
		_saveLoadManager.TryLoadSave(_saveFolder);
		_sceneLoader.TryLoadScene(_saveLoadInfo.CurrentData.SceneName);
    }

    //Used by button
    public void SaveGame()
	{
		_saveLoadManager.SaveDataToFile(_saveFolder, false);
	}

	//Used by button
	public void LoadMainMenu()
	{
		_sceneLoader.TryLoadScene(_mainMenuSceneName);
	}
}
