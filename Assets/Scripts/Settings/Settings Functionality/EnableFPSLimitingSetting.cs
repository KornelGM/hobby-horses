using UnityEngine;

public class EnableFPSLimitingSetting :  ToggleSetting
{
    [SerializeField] private VSyncSetting _vSyncSetting;
    [SerializeField] private GameObject _settingVisualBlocker;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
    }
    private void Update()
    {
        if(_vSyncSetting == null) return;
        
        _settingVisualBlocker.SetActive(_vSyncSetting.CurrentValue);
        SettingToggle.interactable = !_vSyncSetting.CurrentValue;
    }

    public override void Apply()
    {   
        base.Apply();
    }
}