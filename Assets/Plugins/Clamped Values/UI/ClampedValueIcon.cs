using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.UI;

public class ClampedValueIcon : MonoBehaviour
{
    public bool IsVisible { get; private set; } = true;

    [SerializeField] private Image _image;
    [SerializeField] private ParticleImage _filledParticles;
    [SerializeField] private Gradient _gradient;

    private ClampedValue _currentListeningValue;
    private List<Image> _images = new ();

    private void Awake()
    {
        _filledParticles.onParticleFinish.AddListener(CheckObjectActiveness);
        _images = GetComponentsInChildren<Image>().ToList();
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
            _image.enabled = false;
            CheckObjectActiveness();
            return;
        }

        RefreshValues(clampedValue);
        clampedValue.OnValueChanged += OnValueChanged;
        clampedValue.OnReachedMaxValue += OnReachedMaxValue;
    }

    private void RefreshValues(ClampedValue clampedValue)
    {
        float normalized = clampedValue.NormalizedValue();
        
        _image.color = _gradient.Evaluate(normalized);
        _image.fillAmount = normalized;
        
        bool shouldBeEnabled = normalized is < 1 and > 0 && IsVisible;

        foreach (Image image in _images)
        {
            image.enabled = shouldBeEnabled;
        }
        
        CheckObjectActiveness();
    }

    private void OnReachedMaxValue()
    {
        _filledParticles.Play();
    }

    private void CheckObjectActiveness()
    {
        gameObject.SetActive(_image.enabled || _filledParticles.isPlaying);
    }

    private void OnValueChanged(float oldValue, float newValue)
    {
        if (_currentListeningValue == null) return;
        RefreshValues(_currentListeningValue);
    }
    
    public void SetVisibility(bool state)
    {
        IsVisible = state;
        RefreshValues(_currentListeningValue);
    }

    private void OnDestroy()
    {
        if (_currentListeningValue != null)
        {
            _currentListeningValue.OnValueChanged -= OnValueChanged;
            _currentListeningValue.OnReachedMaxValue -= OnReachedMaxValue;
        }
    }
}
