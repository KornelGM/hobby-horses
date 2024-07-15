using UnityEngine;

public abstract class BaseSetting<T> : MonoBehaviour, ISetting<T>, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [HideInInspector] public SettingsTab SettingsTab;
	
    [field: SerializeField] public SettingName Name { get; set; }
    [field: SerializeField] public virtual T CurrentValue { get; set; }
    [field: SerializeField] public virtual T DefaultValue { get; set; }
    [field: SerializeField] public T SavedValue { get; set; }
    [field: SerializeField] public bool CanSetToDefault { get; set; }
	
    public bool HasChanged() => (dynamic)SavedValue != CurrentValue;
    public abstract void SaveSetting();
    public abstract void LoadSetting();
    public virtual void OnValueChanged() => SettingsTab.SettingsWindow.OnValueChanged();
    public virtual void Apply() => SavedValue = CurrentValue;
    public virtual void SetCurrentValue(T value) => CurrentValue = value;
    public virtual void Initialize(SettingsTab tab) => SettingsTab = tab;
}