using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsTab : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }

	[SerializeField] private Transform _parentLayout;
	[field: SerializeField] public List<ISetting<float>> FloatValues { get; private set; } = new();
	[field: SerializeField] public List<ISetting<int>> IntValues { get; private set; } = new();
	[field: SerializeField] public List<ISetting<bool>> BoolValues { get; private set; } = new();

	[field: SerializeField] public List<IProfileSetting<float>> ProfileFloatValues { get; private set; } = new();
	[field: SerializeField] public List<IProfileSetting<int>> ProfileIntValues { get; private set; } = new();
	[field: SerializeField] public List<IProfileSetting<bool>> ProfileBoolValues { get; private set; } = new();
	
	public event Action<SettingsTab> OnSettingsTabOpen;

	public SettingsWindow SettingsWindow;


	private void OnEnable() => OnSettingsTabOpen?.Invoke(this);


	public void Initialize()
	{
		FloatValues = _parentLayout.GetComponentsInChildren<ISetting<float>>().ToList();
		IntValues = _parentLayout.GetComponentsInChildren<ISetting<int>>().ToList();
		BoolValues = _parentLayout.GetComponentsInChildren<ISetting<bool>>().ToList();
		
		ProfileFloatValues = _parentLayout.GetComponentsInChildren<IProfileSetting<float>>().ToList();
		ProfileIntValues = _parentLayout.GetComponentsInChildren<IProfileSetting<int>>().ToList();
		ProfileBoolValues = _parentLayout.GetComponentsInChildren<IProfileSetting<bool>>().ToList();
		
		foreach (var setting in FloatValues) setting.Initialize(this);
		foreach (var setting in IntValues) setting.Initialize(this);
		foreach (var setting in BoolValues) setting.Initialize(this);
	}

	public void SaveTabSettings(string prefix)
	{
		foreach (var setting in FloatValues) setting.SaveSetting();
		foreach (var setting in IntValues) setting.SaveSetting();
		foreach (var setting in BoolValues) setting.SaveSetting();

		SettingFileSaver.SaveToFile();
	}

	public void LoadTabSettings(string prefix)
	{
		SettingFileSaver.LoadFromFile();

		foreach (var setting in FloatValues) setting.LoadSetting();
		foreach (var setting in IntValues) setting.LoadSetting();
		foreach (var setting in BoolValues) setting.LoadSetting();
	}

	public void ApplyTabSettings()
	{
		foreach (var setting in FloatValues) setting.Apply();
		foreach (var setting in IntValues) setting.Apply();
		foreach (var setting in BoolValues) setting.Apply();
	}

	public void SetAllProfileSettingsToProfile(Profiles profile) => 
		SetProfileForEachProfileSetting(profile);


	public void SetAllTabSettingsValuesToDefault()
	{
		foreach (var setting in FloatValues)
		{
			if(!setting.CanSetToDefault) 
				continue;
			
			setting.SetCurrentValue(setting.DefaultValue);
		}
		foreach (var setting in IntValues)
		{
			if(!setting.CanSetToDefault) 
				continue;

			setting.SetCurrentValue(setting.DefaultValue);
		}

		foreach (var setting in BoolValues)
		{
			if(!setting.CanSetToDefault) 
				continue;

			setting.SetCurrentValue(setting.DefaultValue);
		}
	}

	public void RevertTabSettingsValues()
	{
		foreach (var setting in FloatValues) setting.SetCurrentValue(setting.SavedValue);
		foreach (var setting in IntValues) setting.SetCurrentValue(setting.SavedValue);
		foreach (var setting in BoolValues) setting.SetCurrentValue(setting.SavedValue);
	}

	public bool HasChanged()
	{
		foreach (var setting in FloatValues) if (setting.HasChanged()) return true;
		foreach (var setting in IntValues) if (setting.HasChanged()) return true;
		foreach (var setting in BoolValues) if (setting.HasChanged()) return true;
		return false;
	}

	public bool AllSettingsIsDefault()
	{
		foreach (var setting in FloatValues) if (setting.CurrentValue != setting.DefaultValue) return false;
		
		foreach (var setting in IntValues)
		{
			if ( setting is OverallQualitySetting )
				continue;
			
			if (setting.CurrentValue != setting.DefaultValue) return false;
		}
		
		foreach (var setting in BoolValues) if (setting.CurrentValue != setting.DefaultValue) return false;
		return true;
	}

	private void SetProfileForEachProfileSetting( Profiles profile)
	{
		if (ProfileFloatValues.Count > 0)
			foreach (var setting in ProfileFloatValues) SetProfileForSetting(setting, profile);
		
		if (ProfileIntValues.Count > 0)
			foreach (var setting in ProfileIntValues) SetProfileForSetting(setting, profile);
		
		if (ProfileBoolValues.Count > 0)
			foreach (var setting in ProfileBoolValues) SetProfileForSetting(setting, profile);
	}

	private void SetProfileForSetting<T>(IProfileSetting<T> setting, Profiles profile)
	{
		switch (profile)
		{
			case Profiles.Low:
				setting.SetProfile(setting.LowProfileValue);
				break;
			case Profiles.Medium:
				setting.SetProfile(setting.MediumProfileValue);
				break;
			case Profiles.High:
				setting.SetProfile(setting.HighProfileValue);
				break;
			case Profiles.VeryHigh:
				setting.SetProfile(setting.VeryHighProfileValue);
				break;
		}
	}

	public BaseSetting<T> GetBaseSetting<T>(SettingName settingName)
	{
		if (typeof(T) == typeof(float))
		{
			BaseSetting<T> setting = (dynamic)FloatValues.Find(setting => setting.Name == settingName);
			if (setting == null)
			{
				throw new ArgumentException($"Cannot find {settingName} in list Float Values, it must be on one of th sliders");
			}
			return setting;
		}

		if (typeof(T) == typeof(int))
		{
			BaseSetting<T> setting = (dynamic)IntValues.Find(setting => setting.Name == settingName);
			if (setting == null)
			{
				throw new ArgumentException($"Cannot find {settingName} in list Int Values, it must be on one of the dropdowns");
			}
			return setting;

		}

		if (typeof(T) == typeof(bool))
		{
			BaseSetting<T> setting = (dynamic)BoolValues.Find(setting => setting.Name == settingName);
			if (setting == null)
			{
				throw new ArgumentException($"Cannot find {settingName} in list Bool Values, it must be on one of the toggles");
			}
			return setting;
		}

		return default;
	}

	public BaseSetting<T> GetSetting<T>(BaseSetting<T> setting)
	{
		if (setting == null)
		{
			throw new Exception($"Setting {setting} (argument of method) is null");
		}

		if (typeof(T) == typeof(float))
		{
			return (dynamic)GetSliderSetting(setting);
		}

		if (typeof(T) == typeof(int))
		{
			return (dynamic)GetDropdownSetting(setting);
		}

		if (typeof(T) == typeof(bool))
		{
			return (dynamic)GetToggleSetting(setting);
		}

		return default;
	}

	private SliderSetting GetSliderSetting<T>(BaseSetting<T> setting)
	{
		SliderSetting sliderSetting = (dynamic)setting;
		return sliderSetting;
	}

	private ToggleSetting GetToggleSetting<T>(BaseSetting<T> setting)
	{
		ToggleSetting toggleSetting = (dynamic)setting;
		return toggleSetting;
	}

	private DropdownSetting GetDropdownSetting<T>(BaseSetting<T> setting)
	{
		DropdownSetting dropdownSetting = (dynamic)setting;
		return dropdownSetting;
	}

	public void DeleteAllSavedTabSettings()
	{
		FloatValues = _parentLayout.GetComponentsInChildren<ISetting<float>>().ToList();
		IntValues = _parentLayout.GetComponentsInChildren<ISetting<int>>().ToList();
		BoolValues = _parentLayout.GetComponentsInChildren<ISetting<bool>>().ToList();
		
		foreach (var setting in IntValues)
		{
			Debug.Log("Deleting int values");
			PlayerPrefs.DeleteKey(SettingsWindow.SettingsWindowName.ToString() + setting.Name);
		}	
		foreach (var setting in FloatValues)
		{
			Debug.Log("Deleting float values");
			PlayerPrefs.DeleteKey(SettingsWindow.SettingsWindowName.ToString() + setting.Name);
		}	
		foreach (var setting in BoolValues)
		{
			Debug.Log("Deleting bool values");
			PlayerPrefs.DeleteKey(SettingsWindow.SettingsWindowName.ToString() + setting.Name);
		}
		
		FloatValues.Clear();
		IntValues.Clear();
		BoolValues.Clear();
	}
}