using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MaxShadowsDistanceSetting : SliderSetting, IProfileSetting<int>
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;
    [SerializeField] private EnableShadowsSetting _enableShadowsSetting;
    [SerializeField] private GameObject _settingVisualBlocker;

    public int LowProfileValue { get; set; } = 1;
    public int MediumProfileValue { get; set; } = 10;
    public int HighProfileValue { get; set; } = 30;
    public int VeryHighProfileValue { get; set; } = 50;
    public ISetting<int> BaseSetting { get; set; }
    public event Action ValueChanged;
    public void SetProfile(int value)
    {
        SetCurrentValue(value);
    }

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(50, 1);
        SettingSlider.value = SettingSlider.maxValue;
    }

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetMaxDistance(CurrentValue);
    }

    private void Update()
    {
        if(_enableShadowsSetting == null) return;

        _settingVisualBlocker.SetActive(!_enableShadowsSetting.CurrentValue);
        SettingSlider.interactable = _enableShadowsSetting.CurrentValue;
    }

    public void SetMaxDistance(float valueToSet)
    {
        if(_settingsManager == null) return;    
        Volume volume = _settingsManager.GetGlobalVolume();
        if (volume == null) return;

        if (!volume.sharedProfile.TryGet(out HDShadowSettings hdShadowSettings))
        {
            Debug.Log($"Cannot get HDShadowSettings from {volume} ");
            return;
        }

        hdShadowSettings.maxShadowDistance.value = valueToSet;
        ValueChanged?.Invoke();
    }
}