using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    [SerializeField] private string SceneName;
    public void LoadScene() => LoadScene(SceneName);
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  
    }
}
