using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class VignetteSetting : ToggleSetting, IProfileSetting<bool>
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = true;
    public bool HighProfileValue { get; set; } = true;
    public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetVignette();
    }

    private void SetVignette()
    {
        if (_settingsManager == null) return;
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out Vignette vignette))
        {
            Debug.Log($"Cannot get vignette from {globalVolumeFromVariable}");
            return;
        }

        vignette.active = CurrentValue;
        ValueChanged?.Invoke();
    }

    public void SetProfile(bool value)
    {
        SetCurrentValue(value);
    }
}