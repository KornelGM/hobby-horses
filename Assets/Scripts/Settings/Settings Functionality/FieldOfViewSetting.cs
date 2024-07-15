
public class FieldOfViewSetting : SliderSetting
{
    [ServiceLocatorComponent(canBeNull: true)]
    private PlayerManager _playerManager;

    private PlayerCameraServiceLocator _playerCameraServiceLocator;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(100, 40);
        SettingSlider.value = SettingSlider.maxValue;
        if (_playerManager == null) return;

        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerCameraServiceLocator, true);
    }

    public override void Apply()
    {
        base.Apply();
        if (_playerManager == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _playerManager, true)) return;
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerCameraServiceLocator, true);
        }

        SetFieldOfView(CurrentValue);
    }

    private void SetFieldOfView(float currentValue)
    {
        if (_playerCameraServiceLocator == null) return;
        _playerCameraServiceLocator.Camera.m_Lens.FieldOfView = currentValue;
    }
}