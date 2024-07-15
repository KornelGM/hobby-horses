using UnityEngine;

public class LanguageConverter
{
    public string GetDeviceLanguage(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Chinese: return "Simplified Chinese";
            case SystemLanguage.English: return "English";
            case SystemLanguage.French: return "French";
            case SystemLanguage.German: return "German";
            case SystemLanguage.Italian: return "Italian";
            case SystemLanguage.Japanese: return "Japanese";
            case SystemLanguage.Korean: return "Korean";
            case SystemLanguage.Polish: return "Polish";
            case SystemLanguage.Portuguese: return "Portuguese - Brazil";
            case SystemLanguage.Russian: return "Russian";
            case SystemLanguage.Spanish: return "Spanish - Spain";
            case SystemLanguage.Turkish: return "Turkish";
            case SystemLanguage.ChineseSimplified: return "Simplified Chinese";
            case SystemLanguage.ChineseTraditional: return "Traditional Chinese";
            case SystemLanguage.Unknown: return "English";
            default: return "English";
        }
    }
    
    public void Test()
    {
        foreach (SystemLanguage VARIABLE in System.Enum.GetValues(typeof(SystemLanguage)))
        {
            Debug.Log(VARIABLE.ToString() + "        "  + GetDeviceLanguage(VARIABLE));
        }
    }
}
