using Sirenix.OdinInspector;
using UnityEngine;

public class NewGameSetupTool : MonoBehaviour, IServiceLocatorComponent
{
	[SerializeField] private CustomSaveLoadInfo _saveLoadInfo = null;

	[ServiceLocatorComponent] private CustomSaveLoadManager _saveLoadManager;

	public ServiceLocator MyServiceLocator { get; set; }

# if UNITY_EDITOR
	[Button("Reload new game setup")]
    [ContextMenu("Reload new game setup")]
	private void ReloadSetup()
	{
		if (_saveLoadInfo == null)
		{
			Debug.LogError($"{nameof(_saveLoadInfo)} is null");
			return;
		}

		_saveLoadInfo.AddNewSaveDataForScene(_saveLoadManager.CollectInformationsToSaveData());
		UnityEditor.EditorUtility.SetDirty(_saveLoadInfo);
	}
#endif
}
