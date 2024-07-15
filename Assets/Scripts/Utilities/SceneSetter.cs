using UnityEngine;

public class SceneSetter : MonoBehaviour
{
    [SerializeField] private SaveLoadInfo<SaveData> _saveLoadInfo;
    [SerializeField] private string _sceneName;

    public void SetupScene()
    {
        _saveLoadInfo.LoadingScene = _sceneName;
    }
}
