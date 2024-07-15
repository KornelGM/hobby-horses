using System.Collections;
using UnityEngine;

public class QuitGame : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private FadeOutInManager _fadeOutInManager;
    public ServiceLocator MyServiceLocator { get; set; }

    public void QuitGameButton() => QuitGameCoroutine();

    private void QuitGameCoroutine()
    {
        Time.timeScale = 1f;
        System.Action action = () => Application.Quit();
        _fadeOutInManager.StartFadingOutAction(action);
    }
}
