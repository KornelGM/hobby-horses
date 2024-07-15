public class GamepadUISensitivitySetting : SliderSetting
{
    [ServiceLocatorComponent(canBeNull: true)]
    private GamepadCursorManager _cursorManager;
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(20f, 2.5f);
    }
    public override void Apply()
    {
        base.Apply();
        if (_cursorManager == null)
        { 
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _cursorManager, true)) return;
        }
        SetSensitivity(CurrentValue);
    }

    private void SetSensitivity(float currentValue)
    {
        if (_cursorManager == null) return;
        _cursorManager.CursorSensitivity = currentValue;
    }
}