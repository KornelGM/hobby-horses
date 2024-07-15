
public class NotificationsTimeMultiplier : SliderSetting
{
    [ServiceLocatorComponent(canBeNull: true)]
    private NotificationsSystem _notyficationsSystem;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        SetMaxAndMinValuesOfSlider(2, 0.1f);
    }

    public override void Apply()
    {
        base.Apply();
        if (_notyficationsSystem == null)
        {
            if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _notyficationsSystem, true)) return;
        }
        SetNotificationsMultiplier(CurrentValue);
    }

    private void SetNotificationsMultiplier(float currentValue)
    {
        if (_notyficationsSystem == null) return;
        _notyficationsSystem.TimeMultiplier = currentValue;
    }
}