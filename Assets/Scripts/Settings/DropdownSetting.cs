using TMPro;
using UnityEngine;

public class DropdownSetting : BaseSetting<int>
{
	[field: SerializeField] public TMP_Dropdown SettingDropdown { get; private set; }

	public override void Initialize(SettingsTab tab)
	{
		base.Initialize(tab);
		SettingDropdown.onValueChanged.AddListener((x) => OnValueChanged());
	}

	public override void SaveSetting() =>
        SettingFileSaver.SetInt(SettingsTab.SettingsWindow.SettingsWindowName.ToString() + Name.ToString(), SavedValue);

	public override void LoadSetting() => 
		CurrentValue = SettingFileSaver.GetInt(SettingsTab.SettingsWindow.SettingsWindowName.ToString() + Name.ToString(), DefaultValue);

	public override int CurrentValue
	{
		get
		{
			return SettingDropdown.value;
		}
		set
		{
			SettingDropdown.SetValueWithoutNotify(value);
            if (value != SettingDropdown.value) OnValueChanged();
		}
	}
}
