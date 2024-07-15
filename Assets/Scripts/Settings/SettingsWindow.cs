using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : SerializedMonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }

	[ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
	[SerializeField] private SettingsMenuUI _settingsMenuUI;
	[SerializeField] private Button _apply;
	[SerializeField] private Button _reset;
	public SettingsTab _settingsTab;
	[SerializeField] private SettingsWindowName _settingsWindowName;

	[SerializeField] private LocalizedString _applyChangesModalTitle;
	[SerializeField] private LocalizedString _resetToDefaultsModalTitle;
	[SerializeField] private LocalizedString _quitWithoutSavingModalTitle;
	
	public SettingsWindowName SettingsWindowName =>
		_settingsWindowName;

	private void OnEnable()
	{
		OnValueChanged();
		_apply.onClick.AddListener(CreateModalForApplyButton);
		_reset.onClick.AddListener(CreateModalWindowForResetButton);
	}
	
	private void OnDisable()
	{
		_apply.onClick.RemoveListener(CreateModalForApplyButton);
		_reset.onClick.RemoveListener(CreateModalWindowForResetButton);
	}

	public void Initialize()
	{
		_settingsMenuUI.IsNotNull(this,nameof(_settingsMenuUI));
		
		_settingsTab.Initialize();
		_settingsTab.LoadTabSettings(_settingsWindowName.ToString());
		_settingsTab.ApplyTabSettings();
		_settingsTab.SaveTabSettings(_settingsWindowName.ToString());

        OnValueChanged();
        Resources.UnloadUnusedAssets();
	}

	public void CreateModalForApplyButton()
	{
		_modalWindowManager
			.CreateModalWindowYesNo(ApplyButton, () => { },_applyChangesModalTitle);
	}

	public void CreateModalWindowForResetButton()
	{
		_modalWindowManager
			.CreateModalWindowYesNo(ResetSettingsToDefault, () => { },_resetToDefaultsModalTitle);
	}

	public void CreateModalForBackButton(bool mainMenu = false)
	{
		if (!_settingsTab.HasChanged())
		{
			if(mainMenu)
                _settingsMenuUI.CloseSettingsWindowMainMenu();
			else
                _settingsMenuUI.CloseSettingsWindow();

            return;
		}
		_modalWindowManager.CreateModalWindowYesNo(mainMenu 
			? _settingsMenuUI.CloseSettingsWindowMainMenu
            : _settingsMenuUI.CloseSettingsWindow,
			() => { }, _quitWithoutSavingModalTitle);
	}
	
	private void ApplyButton()
	{
        _settingsTab.ApplyTabSettings();
		_settingsTab.SaveTabSettings(_settingsWindowName.ToString());

		OnValueChanged();
        Resources.UnloadUnusedAssets();
    }

	public void OnValueChanged() => 
		_apply.interactable = SettingsChanged();

	private void ResetSettingsToDefault()
	{
		_settingsTab.SetAllTabSettingsValuesToDefault();
		OnValueChanged();
	}
	
	private bool SettingsChanged()
	{
		if (_settingsTab.HasChanged())
		{
            return true;
		}
		return false;
	}
}
