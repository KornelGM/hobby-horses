using System;
using UnityEngine;

public class QualitySetting : DropdownSetting, IProfileSetting<int>
{
    public override void Apply()
    {
        base.Apply();
        SetQuality();
        ValueChanged?.Invoke();
    }

    private void SetQuality() =>
        QualitySettings.SetQualityLevel(CurrentValue);

    public int LowProfileValue { get; set; } = 0;
    public int MediumProfileValue { get; set; } = 1;
    public int HighProfileValue { get; set; } = 2;
    public int VeryHighProfileValue { get; set; } = 2;
    public ISetting<int> BaseSetting { get; set; }
    public event Action ValueChanged;
    public void SetProfile(int value)
    {
        SetCurrentValue(value);
    }
}
