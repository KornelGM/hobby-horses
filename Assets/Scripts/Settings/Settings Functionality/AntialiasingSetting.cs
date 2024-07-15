using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class AntialiasingSetting : DropdownSetting, IProfileSetting<int>
{
    public int LowProfileValue { get; set; } = 0;
    public int MediumProfileValue { get; set; } = 3;
    public int HighProfileValue { get; set; } = 3;
    public int VeryHighProfileValue { get; set; } = 3;
    public ISetting<int> BaseSetting { get; set; }
    public event Action ValueChanged;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        ValueChanged += SettingsTab.SettingsWindow.OnValueChanged;
    }

    public void SetProfile(int value)
    {
        SetCurrentValue(value);
    }
    public override void Apply()
    {
        base.Apply();
        SetAntialiasing();
    }

    public override void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }

    private void SetAntialiasing() =>
        Camera.main.GetComponent<HDAdditionalCameraData>().antialiasing = (HDAdditionalCameraData.AntialiasingMode)CurrentValue;

 
}