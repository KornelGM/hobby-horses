using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ShadowResolutionSetting : DropdownSetting, IProfileSetting<int>
{
    [field: SerializeField] public int LowProfileValue { get; set; }
    [field: SerializeField] public int MediumProfileValue { get; set; }
    [field: SerializeField] public int HighProfileValue { get; set; }
    [field: SerializeField] public int VeryHighProfileValue { get; set; }
    public ISetting<int> BaseSetting { get; set; }
    public event Action ValueChanged;

    [SerializeField] private EnableShadowsSetting _enableShadowsSetting;
    [SerializeField] private GameObject _settingVisualBlocker;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        ValueChanged += SettingsTab.SettingsWindow.OnValueChanged;
        BaseSetting = this;
    }

    private void Update()
    {
        if(_enableShadowsSetting == null) return;
        
        _settingVisualBlocker.SetActive(!_enableShadowsSetting.CurrentValue);
        SettingDropdown.interactable = _enableShadowsSetting.CurrentValue;
    }


    public void SetProfile(int value) => 
        SetCurrentValue(value);

    public override void OnValueChanged() => 
        ValueChanged?.Invoke();

    public override void Apply()
    {
        base.Apply();
        SetShadowsResolution();
    }

    private void SetShadowsResolution()
    {
        switch (CurrentValue)
        {
            case 0:
                SetResolutionForEachLightInScene(512);
                break;
            case 1:
                SetResolutionForEachLightInScene(1024);
                break;
            case 2:
                SetResolutionForEachLightInScene(2048);
                break;
            case 3:
                SetResolutionForEachLightInScene(4096);
                break;
        }
    }

    private void SetResolutionForEachLightInScene(int resolution)
    {
        foreach (HDAdditionalLightData hdAdditionalLight in FindObjectsOfType<HDAdditionalLightData>())
            hdAdditionalLight.SetShadowResolution(resolution);
    }
}