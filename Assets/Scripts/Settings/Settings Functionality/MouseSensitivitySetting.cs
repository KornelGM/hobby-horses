
using UnityEngine;

public class MouseSensitivitySetting : SliderSetting
{
    [SerializeField] private MovementSettings _playerMovementSettings;
    private PlayerCameraServiceLocator _playerCameraServiceLocator;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(1.25f, 0.02f);
      
    }
    public override void Apply()
    {
        base.Apply();
        if (_playerMovementSettings == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _playerMovementSettings, true)) return;
        }
        SetSensitivity(CurrentValue);
    }

    private void SetSensitivity(float currentValue)
    {
        _playerMovementSettings.RotationSpeed = currentValue;
    }
    
}
