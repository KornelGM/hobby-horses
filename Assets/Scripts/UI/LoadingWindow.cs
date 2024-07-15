using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private SaveLoadManager<SaveData, BaseHeaderData> _saveLoadManager;

    [SerializeField] private CustomSaveLoadInfo _saveLoadInfo = null;
    [SerializeField] private Image _loadingPanel;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private Transform _loadingHolder;

    public void OnNewGameSceneLoad(string sceneName = "Chocolate Manufacture")
    {
        if (_saveLoadInfo.GetSaveDataForScene(sceneName) != null)
            _saveLoadManager.ChangeCurrentSave(_saveLoadInfo.GetSaveDataForScene(sceneName));

        LoadScene(sceneName);
    }

    public void OnMainMenuLoad(string sceneName = "Main Menu")
    {
        foreach (var newGameData in _saveLoadInfo.NewGameDatasForScenes)
        {
            newGameData.PlayerSaveData.Statistics.Clear();
        }
        _saveLoadManager.ChangeCurrentSave(null);
        LoadScene(sceneName);
    }

    public void OnSceneLoad(string sceneName)
    {
        LoadScene(sceneName);
    }

    private void LoadScene(string sceneName)
    {
        Resources.UnloadUnusedAssets();

        Time.timeScale = 1f;
        _loadingHolder.gameObject.SetActive(false);
        _loadingPanel.gameObject.SetActive(true);
        _loadingBar.fillAmount= 0;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_loadingPanel.DOFade(1, 1f));
        sequence.AppendCallback(() => _loadingHolder.gameObject.SetActive(true));
        sequence.AppendInterval(0.5f).
            OnComplete(()=>
            {
                StartCoroutine(LoadSceneAsync(sceneName));
            });
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        float progress = 0;

        while (progress < 1)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            _loadingBar.fillAmount = progress;
            
            yield return null;
        }
    }
}
