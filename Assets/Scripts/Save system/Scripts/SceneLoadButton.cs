using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoadButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
	[SerializeField] private string _sceneName;
    [ServiceLocatorComponent] private SceneLoader<SaveData> _sceneLoader;
    [ServiceLocatorComponent] private FadeOutInManager _fadeManager;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=> OnClickButton(_sceneName));
    }

    public void OnClickButton(string sceneName)
    {
        _fadeManager.StartFadingOutAction(() => LoadScene(sceneName));
    }

    public void LoadScene(string sceneName)
    {
       _sceneLoader.TryLoadScene(sceneName);
    }

}
