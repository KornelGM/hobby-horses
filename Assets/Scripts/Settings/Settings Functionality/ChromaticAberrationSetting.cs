using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ChromaticAberrationSetting : ToggleSetting
{
    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return;
        }
        SetChromaticAberration();
    }

    private void SetChromaticAberration()
    {
        Volume globalVolumeFromVariable = _settingsManager.GetGlobalVolume();
        
        if (!globalVolumeFromVariable.sharedProfile.TryGet(out ChromaticAberration chromaticAberration))
        {
            Debug.Log($"Cannot get {nameof(chromaticAberration)} from {globalVolumeFromVariable}");
            return;
        }

        chromaticAberration.active = CurrentValue;
    }
}