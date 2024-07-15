using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BloomSetting : ToggleSetting, IProfileSetting<bool>
{
    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = false;
    public bool HighProfileValue { get; set; } = true;
    public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;

    [ServiceLocatorComponent] private SettingsManager _settingsManager;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        BaseSetting = this;
        ValueChanged += SettingsTab.SettingsWindow.OnValueChanged;
    }

    public void SetProfile(bool value) => 
        SetCurrentValue(value);

    public override void OnValueChanged() => 
        ValueChanged?.Invoke();

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetBloom();
    }

    private void SetBloom()
    {
        VolumeProfile defaultProfile = _settingsManager.GetDefaultProfile();

        if (!defaultProfile.TryGet(out Bloom bloom))
        {
            Debug.Log($"Cant get bool from {defaultProfile}");
            return;
        }

        bloom.active = CurrentValue;
    }
}