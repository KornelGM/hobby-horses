using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DepthOfFieldSetting : ToggleSetting, IProfileSetting<bool>
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;
    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = false;
    public bool HighProfileValue { get; set; } = false;
    public bool VeryHighProfileValue { get; set; } = true;
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

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        { 
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetDepthOfField();
    }

    private void Update()
    {
        _settingBlocker.SetActive(_qualitySetting.CurrentValue < minQualitySetting);
        SettingToggle.interactable = !(_qualitySetting.CurrentValue < minQualitySetting);
        if (_qualitySetting.CurrentValue < minQualitySetting) CurrentValue = false;
    }

    private void SetDepthOfField()
    {
        if (_settingsManager == null) return;
        Volume volume = _settingsManager.GetGlobalVolume();
        
        if (!volume.sharedProfile.TryGet(out DepthOfField depthOfField))
        {
            Debug.Log($"Cannot get {nameof(depthOfField)} from {volume}");
            return;
        }

        depthOfField.active = CurrentValue;
    }

    public override void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }
    
    public void SetProfile(bool value)
    {
        SetCurrentValue(value);
    }
}