using UnityEngine;
using UnityEngine.UI;

public class ToggleSetting : BaseSetting<bool>
{
	[field: SerializeField] public Toggle SettingToggle { get; private set; }

	public override void Initialize(SettingsTab tab)
	{
		base.Initialize(tab);
		SettingToggle.onValueChanged.AddListener((x) => OnValueChanged());
	}

	public override void SaveSetting() =>
        SettingFileSaver.SetInt(SettingsTab.SettingsWindow.SettingsWindowName + Name.ToString(), SavedValue ? 1 : 0);

	public override void LoadSetting()
	{
        CurrentValue = SettingFileSaver.GetBool(SettingsTab.SettingsWindow.SettingsWindowName.ToString() + Name.ToString(), DefaultValue);
	}

	public override bool CurrentValue
	{
		get
		{
			return SettingToggle.isOn;
		}
		set
		{
			SettingToggle.SetIsOnWithoutNotify(value);
			OnValueChanged();
		}
	}
}
