using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GoToScene))]
public class BackToMenu : MonoBehaviour
{
    private GoToScene _goToScene;

    private void Start()
    {
        _goToScene = GetComponent<GoToScene>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _goToScene.LoadScene();
        }
    }
}
