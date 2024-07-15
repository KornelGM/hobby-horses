using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SaturationSetting : SliderSetting
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(60f, -60f);
    }

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetSaturation();
    }

    private void SetSaturation()
    {
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();

        if (globalVolumeFromVariable == null) return;


        if (!globalVolumeFromVariable.sharedProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Debug.LogError("Cannot get color adjustments, cannot change saturation");
            return;
        }

        colorAdjustments.saturation.value = CurrentValue;
    }
}