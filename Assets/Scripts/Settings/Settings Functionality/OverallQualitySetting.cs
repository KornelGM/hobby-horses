using System.Linq;

public class OverallQualitySetting : DropdownSetting
{
    private bool _valueOfDropdownLocked = false;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
    }

    public override void OnValueChanged()
    {
        base.OnValueChanged();
        _valueOfDropdownLocked = true;
        SetProfile();
        _valueOfDropdownLocked = false;
    }

    public override void SetCurrentValue(int value)
    {
        if (_valueOfDropdownLocked) return;
        base.SetCurrentValue(value);
    }

    private void CheckProfiles()
    {
        if (CheckProfileLow())
        {
            SetCurrentValue(0);
            return;
        }

        if (CheckProfileMedium())
        {
            SetCurrentValue(1);
            return;
        }

        if (CheckProfileHigh())
        {
            SetCurrentValue(2);
            return;
        }

        if (CheckProfileVeryHigh())
        {
            SetCurrentValue(3);
            return;
        }

        SetCurrentValue(4);
    }


    private void SetProfile()
    {
        switch (CurrentValue)
        {
            case 0:
                SettingsTab.SetAllProfileSettingsToProfile(Profiles.Low);
                break;
            case 1:
                SettingsTab.SetAllProfileSettingsToProfile(Profiles.Medium);
                break;
            case 2:
                SettingsTab.SetAllProfileSettingsToProfile(Profiles.High);
                break;
            case 3:
                SettingsTab.SetAllProfileSettingsToProfile(Profiles.VeryHigh);
                break;
        }
    }


    private bool CheckProfileLow()
    {
        return SettingsTab.ProfileFloatValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.LowProfileValue)
               &&
               SettingsTab.ProfileIntValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.LowProfileValue)
               &&
               SettingsTab.ProfileBoolValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.LowProfileValue);
    }

    private bool CheckProfileMedium()
    {
        return SettingsTab.ProfileFloatValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.MediumProfileValue)
               &&
               SettingsTab.ProfileIntValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.MediumProfileValue)
               &&
               SettingsTab.ProfileBoolValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.MediumProfileValue);
    }

    private bool CheckProfileHigh()
    {
        return SettingsTab.ProfileFloatValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.HighProfileValue)
               &&
               SettingsTab.ProfileIntValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.HighProfileValue)
               &&
               SettingsTab.ProfileBoolValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.HighProfileValue);
    }

    private bool CheckProfileVeryHigh()
    {
        return SettingsTab.ProfileFloatValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.VeryHighProfileValue)
               &&
               SettingsTab.ProfileIntValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.VeryHighProfileValue)
               &&
               SettingsTab.ProfileBoolValues.All(profileSettings =>
                   profileSettings.BaseSetting.CurrentValue == profileSettings.VeryHighProfileValue);
    }
}