using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ExposureSetting : SliderSetting
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(4, -4);
    }

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetExposure();
    }

    private void SetExposure()
    {
        if (_settingsManager == null) return;
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if(globalVolumeFromVariable == null) return;
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Debug.LogError("Cannot get color adjustments, cannot change exposure");
            return;
        }

        colorAdjustments.postExposure.value = CurrentValue;
    }
}