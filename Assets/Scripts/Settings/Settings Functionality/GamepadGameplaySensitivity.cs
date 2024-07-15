using UnityEngine;

public class GamepadGameplaySensitivity : SliderSetting
{
    [SerializeField] private MovementSettings _playerMovementSettings;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(5.0f, 0.1f);
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
        if(_playerMovementSettings == null) return;
        _playerMovementSettings.GamepadRotationSpeedMultiplier = currentValue;
    }
}