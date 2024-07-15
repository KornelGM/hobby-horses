using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MicroShadowSetting : ToggleSetting, IProfileSetting<bool>
{
    
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = false;
    public bool HighProfileValue { get; set; } = true;
    public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;
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
        SetMicroShadows();
    }

    private void SetMicroShadows()
    {
        if (_settingsManager == null) return;
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out MicroShadowing microShadowing))
        {
            Debug.Log($"Cannot get {microShadowing} from {globalVolumeFromVariable}");
            return;
        }

        microShadowing.active = CurrentValue;
        ValueChanged?.Invoke();
    }
}
