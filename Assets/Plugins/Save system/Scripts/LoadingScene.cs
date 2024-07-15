using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene<T,N> : SerializedMonoBehaviour
    where T : class, new()
	where N : new()
{
	public static LoadingScene<T,N> Instance = null;

	[OdinSerialize] private SaveLoadManager<T, N> _saveLoadManager;

	[SerializeField] private LoadingCanvas _loadingCanvas = null;
	[SerializeField] private SaveLoadInfo<T> _saveLoadInfo = null;
	void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	private void Start()
	{
		StartCoroutine(LoadAsyncScene(_saveLoadInfo.LoadingScene));
	}

	IEnumerator LoadAsyncScene(string name)
	{
		_loadingCanvas.gameObject.SetActive(true);

		Application.backgroundLoadingPriority = ThreadPriority.Low;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

		asyncLoad.allowSceneActivation = false;
		asyncLoad.completed += (e) => TryLoad(name);

		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f)
			{
				asyncLoad.allowSceneActivation = true;
			}

			yield return null;
		}

		SceneManager.UnloadSceneAsync(SaveSystemConstantInfo.LoadingSceneName);
	}

	private void TryLoad(string name)
	{
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        //_saveLoadManager.ChangeCurrentSave(_saveLoadInfo.CurrentData);
	}
}
