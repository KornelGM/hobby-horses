using UnityEngine;

public class VSyncSetting : ToggleSetting
{
    [SerializeField] private QualitySetting _qualitySetting;
    [SerializeField] private int minQualitySetting = 1;
    [SerializeField] private GameObject _settingBlocker;
    [SerializeField] private bool _disableAccordingToOtherSettings = false;

    public override void Apply()
    {
        base.Apply();
        SetVSync();
    }

    private void Update()
    {
        if (!_disableAccordingToOtherSettings) return;
        _settingBlocker.SetActive(_qualitySetting.CurrentValue < minQualitySetting);
        SettingToggle.interactable = !(_qualitySetting.CurrentValue < minQualitySetting);
        if (_qualitySetting.CurrentValue < minQualitySetting) CurrentValue = false;
    }

    private void SetVSync() =>
        QualitySettings.vSyncCount = SavedValue ? 1 : 0;
}
