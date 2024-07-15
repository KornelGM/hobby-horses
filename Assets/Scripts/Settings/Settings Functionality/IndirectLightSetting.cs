using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class IndirectLightSetting : ToggleSetting, IProfileSetting<bool>
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = true;
    public bool HighProfileValue { get; set; } = true;
    public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;
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
        SetIndirectLight();
    }

    private void SetIndirectLight()
    {
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out IndirectLightingController indirectLightingController))
        {
            Debug.Log($"Cannot get {nameof(indirectLightingController)} from {globalVolumeFromVariable}");
            return;
        }

        indirectLightingController.active = CurrentValue;
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