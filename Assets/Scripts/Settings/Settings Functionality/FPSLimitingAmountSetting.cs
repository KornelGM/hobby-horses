using UnityEngine;

public class EnableFPSLimitingAmountSetting : SliderSetting
{
    [SerializeField] private VSyncSetting _vSyncSetting;
    [SerializeField] private EnableFPSLimitingSetting _fpsLimitingEnable;

    [SerializeField] private GameObject _settingVisualBlocker;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(300, 1);
    }

    private void Update()
    {
        if (_vSyncSetting == null) return;

        _settingVisualBlocker.SetActive(_vSyncSetting.CurrentValue || !_fpsLimitingEnable.CurrentValue);
        SettingSlider.interactable = !_vSyncSetting.CurrentValue && _fpsLimitingEnable.CurrentValue;
    }

    public override void Apply()
    {
        base.Apply();
        Application.targetFrameRate =
            _fpsLimitingEnable.CurrentValue && !_vSyncSetting.CurrentValue ? (int)CurrentValue : -1;
    }
}