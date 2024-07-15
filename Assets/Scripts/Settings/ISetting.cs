public interface ISetting<T>
{
    public SettingName Name { get; set; }
    public T CurrentValue { get; set; }
    public T DefaultValue { get; set; }
    public T SavedValue { get; set; }
    public bool CanSetToDefault { get; set; }
    public void OnValueChanged();
    public void SetCurrentValue(T value);
    public void Apply();
    public void LoadSetting();
    public void Initialize(SettingsTab tab);
    public bool HasChanged();
    public void SaveSetting();
}