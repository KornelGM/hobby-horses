using UnityEngine;
using UnityEngine.Audio;

public class VolumeSetting : SliderSetting
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _volumeParameter;

    public override void Initialize(SettingsTab tab)
    {
        _audioMixer.IsNotNull(this, nameof(_audioMixer));
        base.Initialize(tab);
    }

    public override void OnValueChanged()
    {
        base.OnValueChanged();
        SetMixersVolume(CurrentValue);
    }

    public override void SaveSetting()
    {
        base.SaveSetting();
        SetMixersVolume(CurrentValue);
    }

    private void SetMixersVolume(float value)
    {
        float decybells = VolumeInDecyBells(value);
        _audioMixer.SetFloat(_volumeParameter, decybells);
    }

    private float VolumeInDecyBells(float sliderValue)
    {
        if (sliderValue == 0) return -80;

        return Mathf.Log10(sliderValue) * 20;
    }
}
