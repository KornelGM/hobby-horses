using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneLoader<T> : MonoBehaviour
	where T : class
{
	protected abstract SaveLoadInfo<T> _saveLoadInfo { get;}

	public void TryLoadScene(string name)
	{
        Time.timeScale = 1f;
        _saveLoadInfo.LoadingScene = name;
		StartCoroutine(LoadAsyncScene(name));
	}

	private IEnumerator LoadAsyncScene(string name)
	{
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;

		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f)
			{
				asyncLoad.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
