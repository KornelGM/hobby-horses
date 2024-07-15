using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionSetting : DropdownSetting
{
    private DropdownSetting _screenModeSetting;

    public Resolution[] Resolutions 
        => Screen.resolutions;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        FindScreenModeSetting();
        FillUpResolutionDropdown();
        SetDefaultValue(Screen.currentResolution);
    }
    
    public override void Apply()
    {
        base.Apply();
        SetResolution(Resolutions[CurrentValue]);
    }

    public void MatchToAnotherDisplay()
    {
        FillUpResolutionDropdown();
        
        int indexOfResolution = FindIndexOfResolution(Screen.currentResolution);
        
        SetDefaultValue(Screen.currentResolution);
        SetCurrentValue(indexOfResolution);
        
        CurrentValue = indexOfResolution;
        SettingDropdown.value = indexOfResolution;
        
        Apply();

        SaveSetting();
    } 
    
    private void SetResolution(Resolution resolution) => 
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)_screenModeSetting.SavedValue, resolution.refreshRateRatio);
    
    private Resolution SetDefaultValue(Resolution resolution)
    {
        int indexOfCurrentScreenResolution = FindIndexOfResolution(resolution);
        DefaultValue = indexOfCurrentScreenResolution;
        
        return Resolutions[DefaultValue];
    }
    
    private int FindIndexOfResolution(Resolution resolution)
    {
        List<int> list = Resolutions.Select((res, i) => new { index = i, resolution = res }).
            Where(indexAndResolution => indexAndResolution.resolution.Equals(resolution)).
            Select(indexAndResolution => indexAndResolution.index).
            ToList();

        int index = list.Count == 0 
            ? 0 
            : list[0];

        return index;
    }

    private void FillUpResolutionDropdown()
    {
        List<string> resolutions = new();

        foreach (Resolution resolution in Resolutions)
        {
            resolutions.Add(resolution.ToString());
        }
        
        SettingDropdown.ClearOptions();
        SettingDropdown.AddOptions(resolutions);
    }

    private void FindScreenModeSetting() => 
        _screenModeSetting = (DropdownSetting)SettingsTab.GetSetting(SettingsTab.GetBaseSetting<int>(SettingName.ScreenMode));
}