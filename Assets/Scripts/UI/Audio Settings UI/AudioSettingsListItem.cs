using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsListItem : MonoBehaviour
{
    [SerializeField] private string _volumeParameter;
    [SerializeField] AudioMixer _mixer;   
    [SerializeField] Slider _slider;
    [SerializeField] float _multiplier;
    [SerializeField] Toggle _toggle;
    [SerializeField] Image _toggleImage;
    [SerializeField] Sprite _toggleOnSprite, _toggleOffSprite;

    private void Awake()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
        _toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        _mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * _multiplier);
        _toggle.isOn = _slider.value > _slider.minValue;
        ToggleSpriteSwap(_toggle.isOn);
    }

    private void HandleToggleValueChanged(bool enableSound) => _slider.value = enableSound ? 0.75f : _slider.minValue;
    private void ToggleSpriteSwap(bool enableSound) => _toggleImage.sprite = enableSound ? _toggleOnSprite : _toggleOffSprite;
}
