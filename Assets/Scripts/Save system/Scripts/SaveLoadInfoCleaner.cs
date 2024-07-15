using UnityEngine;

public class SaveLoadInfoCleaner : MonoBehaviour
{
    public static SaveLoadInfoCleaner Instance = null;
    [SerializeField] CustomSaveLoadInfo _saveLoadInfo;

    private void Awake()
    {
        if(Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            Instance = this;
            _saveLoadInfo.CurrentData = null;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
