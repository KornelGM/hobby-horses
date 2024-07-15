using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class VisualsController : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _visualEffects;

    public void SetVisualParameter(string parameterName, float value, float duration = 1f, UnityEvent onEndChange = null)
    {
        foreach (var effect in _visualEffects)
        {
            if (!effect.HasFloat(parameterName)) continue;

            float currentValue = effect.GetFloat(parameterName);
            DOTween.To(() => currentValue, x => currentValue = x, value, duration)
                .OnUpdate(() => effect.SetFloat(parameterName, currentValue)).OnComplete(() => onEndChange?.Invoke());
        }
    }

    public void SetVisualParameter(string parameterName, int value, float duration = 1f, UnityEvent onEndChange = null)
    {
        foreach (var effect in _visualEffects)
        {
            if (!effect.HasInt(parameterName)) continue;

            int currentValue = effect.GetInt(parameterName);
            DOTween.To(() => currentValue, x => currentValue = x, value, duration)
                .OnUpdate(() => effect.SetInt(parameterName, currentValue)).OnComplete(() => onEndChange?.Invoke());
        }
    }

    public void SetVisualParameter(string parameterName, bool value, UnityEvent onEndChange = null)
    {
        foreach (var effect in _visualEffects)
        {
            if (!effect.HasBool(parameterName)) continue;

            effect.SetBool(parameterName, value);
            onEndChange?.Invoke();
        }
    }

    public void OneShot(string parameterName, float value, float duration)
    {
        UnityEvent onEnd = new();
        onEnd.AddListener(() => SetVisualParameter(parameterName, 0, duration));
        SetVisualParameter(parameterName, value, duration, onEnd);
    }

    public void OneShot(string parameterName, int value, float duration)
    {
        UnityEvent onEnd = new();
        onEnd.AddListener(() => SetVisualParameter(parameterName, 0, duration));
        SetVisualParameter(parameterName, value, duration, onEnd);
    }

    private void OnDestroy()
    {
        // DOTween.CompleteAll();
    }
}
