using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WhiteBalanceSetting : ToggleSetting
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public override void Apply()
    {
        base.Apply();
        SetWhiteBalance();
    }

    private void SetWhiteBalance()
    {
        if(_settingsManager == null) return;
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out WhiteBalance whiteBalance))
        {
            Debug.Log($"Cannot get {nameof(WhiteBalance)} from {globalVolumeFromVariable}");
            return;
        }

        whiteBalance.active = CurrentValue;
    }
}