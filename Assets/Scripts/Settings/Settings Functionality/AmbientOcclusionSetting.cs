using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class AmbientOcclusionSetting : ToggleSetting, IProfileSetting<bool>
{
    public bool LowProfileValue { get; set; } = false;
    public bool MediumProfileValue { get; set; } = true;
    public bool HighProfileValue { get; set; } = true;
    public bool VeryHighProfileValue { get; set; } = true;
    public ISetting<bool> BaseSetting { get; set; }
    public event Action ValueChanged;

    [SerializeField] private QualitySetting _qualitySetting;
    [SerializeField] private int minQualitySetting = 1;
    [SerializeField] private GameObject _settingBlocker;

    [ServiceLocatorComponent(canBeNull:true)] private SettingsManager _settingsManager;

    public override void Apply()
    {
        base.Apply();
        if (_settingsManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _settingsManager, true)) return; ;
        }
        SetAmbientOcclusion();
    }

    private void Update()
    {
        _settingBlocker.SetActive(_qualitySetting.CurrentValue < minQualitySetting);
        SettingToggle.interactable = !(_qualitySetting.CurrentValue < minQualitySetting);
        if (_qualitySetting.CurrentValue < minQualitySetting) CurrentValue = false;
    }

    private void SetAmbientOcclusion()
    {
        if(_settingsManager == null) return;
        Volume volume = _settingsManager.GetGlobalVolume();

        if (volume == null) return;

        volume.sharedProfile.TryGet(out ScreenSpaceAmbientOcclusion occlusion);
        occlusion.active = SavedValue;
        ValueChanged?.Invoke();
    }

    public void SetProfile(bool value)
    {
        SetCurrentValue(value);
    }
}