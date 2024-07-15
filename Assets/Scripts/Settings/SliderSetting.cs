using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : BaseSetting<float>
{
	[field: SerializeField] public Slider SettingSlider;

	public override void Initialize(SettingsTab tab)
	{
		base.Initialize(tab);
		SettingSlider.onValueChanged.AddListener((x) => OnValueChanged());
	}

	public override void SetCurrentValue(float value)
	{
		base.SetCurrentValue(value);
		SettingSlider.onValueChanged.Invoke(value);
	}

	public override void SaveSetting() => 
		SettingFileSaver.SetFloat(SettingsTab.SettingsWindow.SettingsWindowName.ToString() + Name.ToString(), SavedValue);

	public override void LoadSetting() => 
		CurrentValue = SettingFileSaver.GetFloat(SettingsTab.SettingsWindow.SettingsWindowName.ToString() + Name.ToString(), DefaultValue);

	public override float CurrentValue
	{
		get => SettingSlider.value;
		set
		{
			SettingSlider.SetValueWithoutNotify(value);
            SettingSlider.onValueChanged.Invoke(value);
        }
	}

	public void SetMaxAndMinValuesOfSlider(float maximumValue, float minimumValue)
	{
		SettingSlider.maxValue = maximumValue;
		SettingSlider.minValue = minimumValue;
	}
}