using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ClampedValueSlider : MonoBehaviour
{
    private ClampedValue _currentListeningValue;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void Initialize(ClampedValue clampedValue)
    {
        if (_currentListeningValue != null)
        {
            _currentListeningValue.OnValueChanged -= OnValueChanged;
            _currentListeningValue.OnReachedMaxValue -= OnReachedMaxValue;
        }

        _currentListeningValue = clampedValue;
        if(clampedValue == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        RefreshValues(clampedValue);
        clampedValue.OnValueChanged += OnValueChanged;
        clampedValue.OnReachedMaxValue += OnReachedMaxValue;
    }

    private void RefreshValues(ClampedValue clampedValue)
    {
        _slider.maxValue = clampedValue.MaxValue;
        _slider.minValue = clampedValue.MinValue;
        _slider.value = clampedValue.Value;
    }

    private void OnReachedMaxValue()
    {
        Debug.LogWarning("Progress bar reached max value, here will be feedback for player");
    }

    private void OnSliderValueChanged(float value)
    {
        if (_currentListeningValue == null) return;

        _currentListeningValue.Value = value;
    }

    private void OnValueChanged(float oldValue, float newValue)
    {
        _slider.value = newValue;
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        if (_currentListeningValue != null)
        {
            _currentListeningValue.OnValueChanged -= OnValueChanged;
            _currentListeningValue.OnReachedMaxValue -= OnReachedMaxValue;
        }
    }
}
