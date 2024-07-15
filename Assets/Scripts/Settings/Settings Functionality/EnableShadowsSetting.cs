using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
public class EnableShadowsSetting : ToggleSetting, IProfileSetting<bool>
{
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
        SetShadowsForEachLightInScene(CurrentValue);
    }

    private void SetShadowsForEachLightInScene(bool enabled)
    {
        foreach (HDAdditionalLightData hdAdditionalLight in FindObjectsOfType<HDAdditionalLightData>())
        {
            if (hdAdditionalLight.GetComponent<Light>().type == LightType.Directional) continue;
            
            hdAdditionalLight.EnableShadows(enabled);
        }
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