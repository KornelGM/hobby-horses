using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

public class LanguageSetting : DropdownSetting
{
    [SerializeField] private bool _onlyEnglish = false;
    
    public override int DefaultValue => GetDefaultLanguage();
    
    private List<string> _languages;
    private LanguageConverter _languageConverter = new();

    [Button]
    private void Debug()
    {
        _languageConverter.Test();
    }
    
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        FillUpLanguagesDropDown();
    }
    
    public override void Apply()
    {
        base.Apply();
        LocalizationManager.CurrentLanguage = _languages[SettingDropdown.value];
    }
    
    private void FillUpLanguagesDropDown()
    {
        if (_onlyEnglish)
        {
            _languages = new() { "English" };
        }
        else
        {
            _languages = LocalizationManager.GetAllLanguages();
        }
        
        SettingDropdown.ClearOptions();
        SettingDropdown.AddOptions(_languages);
    }

    public int GetDefaultLanguage()
    {
        List<string> languages = LocalizationManager.GetAllLanguages();

        string foundLanguage = languages.Find(language =>
            language == _languageConverter.GetDeviceLanguage(Application.systemLanguage));

        if (!string.IsNullOrEmpty(foundLanguage)) return languages.IndexOf(foundLanguage);
        return 1;
    }

}
