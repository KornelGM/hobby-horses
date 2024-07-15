using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using I2.Loc;

public class ScreenModeSetting : DropdownSetting
{
    [SerializeField] LocalizeDropdown _localizeDropdown;
    [SerializeField] string _localizePrefix = "";

    private DropdownSetting _resolutionDropdownSetting;
    private ResolutionSetting _resolutionSetting;

    private List<string> _screenModes =>
        System.Enum.GetNames(typeof(FullScreenMode)).ToList();


    public override void Initialize(SettingsTab tab)
    {
        //FillUpDropdown();
        base.Initialize(tab);
        FindResolutionDropdownSetting();
        GetResolutionSetting();
    }

    public override void Apply()
    {
        base.Apply();
        SetScreenMode();
        SaveSetting();
    }

    private void SetScreenMode()
    {
        Resolution resolution = _resolutionDropdownSetting.SavedValue >= _resolutionSetting.Resolutions.Length 
            ? _resolutionSetting.Resolutions[^1]
            : _resolutionSetting.Resolutions[_resolutionDropdownSetting.SavedValue];
        
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)SavedValue, resolution.refreshRateRatio);
    }

    private void FindResolutionDropdownSetting() =>
        _resolutionDropdownSetting = (DropdownSetting)SettingsTab.GetSetting(SettingsTab.GetBaseSetting<int>(SettingName.Resolution));

    private void GetResolutionSetting() =>
        _resolutionSetting = (ResolutionSetting)_resolutionDropdownSetting;

    private void FillUpDropdown()
    {
        _screenModes.Reverse();
        
        SettingDropdown.ClearOptions();
        SettingDropdown.AddOptions(_screenModes);

        //LocalizeDropdown();
    }

    private void LocalizeDropdown()
    {
        _localizeDropdown._Terms.Clear();

        foreach (string screenMode in _screenModes)
        {
            _localizeDropdown._Terms.Add($"{_localizePrefix}{screenMode}");
        }
        
        _localizeDropdown.UpdateLocalization();
    }
}