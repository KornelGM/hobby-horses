using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class FadeOutIn : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _loadText;
    public float FadeInDuration { get; private set; } = 2f;
    public float FadeOutDuration { get; private set; } = 2f;

    public ServiceLocator MyServiceLocator { get; set; }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_image.color.a == 0) return;
        _windowManager.SetMainCanvasOrder();
        StartCoroutine(FadingIn());
    }

    public IEnumerator FadingOut(Action onComplete = null, Func<bool> ableToContinue = null)
    {
        _windowManager.SetMainCanvasOrder();
        _image.raycastTarget = true;

        float elapsedTime = 0f;
        while (elapsedTime < FadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / FadeInDuration);
            SetTransparency(alpha);
            yield return null;
        }

        SetTransparency(1f);

        while (ableToContinue != null && !ableToContinue())
        {
            yield return null;
        }

        onComplete?.Invoke();
    }

    public IEnumerator FadingIn(Action onComplete = null)
    {
        SetTransparency(1f);
        _loadText?.gameObject.SetActive(false);
        float elapsedTime = 0f;
        yield return new WaitForSeconds(1f);

        while (elapsedTime < FadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(255f, 0f, elapsedTime / FadeOutDuration) / 255f;
            SetTransparency(alpha);
            yield return null;
        }
        SetTransparency(0f);
        _image.raycastTarget = false;

        onComplete?.Invoke();
    }

    private void SetTransparency(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}
