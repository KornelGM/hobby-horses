using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MotionBlurSetting : ToggleSetting, IProfileSetting<bool>
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;
    [field: SerializeField] public bool LowProfileValue { get; set; } = false;
    [field: SerializeField] public bool MediumProfileValue { get; set; } = false;
    [field: SerializeField] public bool HighProfileValue { get; set; } = true;
    [field: SerializeField] public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;

    [SerializeField] private QualitySetting _qualitySetting;
    [SerializeField] private int minQualitySetting = 2;
    [SerializeField] private GameObject _settingBlocker;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        ValueChanged += SettingsTab.SettingsWindow.OnValueChanged;

    }

    private void Update()
    {
        _settingBlocker.SetActive(_qualitySetting.CurrentValue < minQualitySetting);
        SettingToggle.interactable = !(_qualitySetting.CurrentValue < minQualitySetting);
        if (_qualitySetting.CurrentValue < minQualitySetting) CurrentValue = false;

    }

    public void SetProfile(bool value)
    {
        SetCurrentValue(value);
    }

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetMotionBlur();
    }

    public void SetMotionBlur()
    {
        if(_settingsManager == null) return;

        _settingsManager.GetGlobalVolume().sharedProfile.TryGet(out MotionBlur motionBlur);
        if (motionBlur == null)
        {
            Debug.Log("Could not find motion blur on shared profile.");
            return;
        }
        
        motionBlur.active = CurrentValue;
    }
    public override void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }

}
