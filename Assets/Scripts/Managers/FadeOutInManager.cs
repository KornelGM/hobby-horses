using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeOutInManager : MonoBehaviour, IManager, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private FadeOutIn _fadeOutIn;

    void OnEnable() => StartFadingInAction();

    ////void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _fadeOutIn = FindObjectOfType<FadeOutIn>();

    //void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /// <summary>
    /// Make screen dark
    /// </summary>
    /// <returns></returns>
    public void StartFadingOutAction(Action onComplete = null, Func<bool> waitUntil = null) => StartCoroutine(_fadeOutIn.FadingOut(onComplete, waitUntil));

    /// <summary>
    /// make screen light
    /// </summary>
    /// <returns></returns>
    public void StartFadingInAction(Action onComplete = null) => StartCoroutine(_fadeOutIn.FadingIn(onComplete));

    /// <summary>
    /// Make screen dark
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartFadingOut()
    {
        yield return StartCoroutine(_fadeOutIn.FadingOut());
    }

    /// <summary>
    /// make screen light
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartFadingIn()
    {
        yield return StartCoroutine(_fadeOutIn.FadingIn());
    }
}
