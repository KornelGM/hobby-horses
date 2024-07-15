using I2.Loc;
using System.Collections.Generic;

public static class TranslationKeys
{
    #region Prefix

    public static string NotificationPrefix = "Notifications/";
    public static string GeneralPrefix = "General/";
    public static string TooltipsPrefix = "Tooltips/";
    public static string MenuPrefix = "Menu/";
    public static string ItemsPrefix = "Items/";
    public static string AdditionalPrefix = "Additional/";
    public static string LeaguePrefix = GeneralPrefix + "League/";
    public static string CategoryPrefix = GeneralPrefix + "Category/";
    public static string DifficultyPrefix = GeneralPrefix + "Difficulty/";

    #endregion

    #region Menu

    public static LocalizedString ModalYesButton = new(MenuPrefix + "Modals/YesButton");
    public static LocalizedString ModalNoButton = new(MenuPrefix + "Modals/NoButton");
    public static LocalizedString ModalConfirmButton = new(MenuPrefix + "Modals/ConfirmButton");
    public static LocalizedString ModalOkButton = new(MenuPrefix + "Modals/OkButton");

    #endregion

    #region Notifications



    #endregion

    #region Tooltips

    public static LocalizedString TooltipsTakeItemTo = new (TooltipsPrefix + "Take item to");    
    public static LocalizedString TooltipsUseItemOn = new (TooltipsPrefix + "Use item on");
    public static LocalizedString TooltipsControlItem = new (TooltipsPrefix + "Control item");
    public static LocalizedString TooltipsUseItem = new (TooltipsPrefix + "Use item");
    public static LocalizedString TooltipsTightenScrews = new (TooltipsPrefix + "Tighten screws");

    #endregion

    #region General
    public static LocalizedString Closed = new (GeneralPrefix + "Closed");
    public static LocalizedString OK = new (GeneralPrefix + "OK");
    public static LocalizedString Amount = new (GeneralPrefix + "Amount");
    public static LocalizedString TotalAmount = new (GeneralPrefix + "TotalAmount");
    public static LocalizedString Empty = new (GeneralPrefix + "Empty");
    public static LocalizedString NotAvailable = new (GeneralPrefix + "NotAvailable");
    public static LocalizedString LeftIn = new (GeneralPrefix + "Left in");
    public static LocalizedString NoOrder = new (GeneralPrefix + "No order");
    public static LocalizedString Difficulty = new (GeneralPrefix + "Difficulty");
    public static LocalizedString FillLevel = new (GeneralPrefix + "Fill level");
    public static LocalizedString NoSpaceInInventory = new (GeneralPrefix + "No space in inventory");
    public static LocalizedString MatchingRecipeFound = new (GeneralPrefix + "Matching recipe found");
    public static LocalizedString Yes = new (GeneralPrefix + "Yes");
    public static LocalizedString No = new (GeneralPrefix + "No");
    public static LocalizedString On = new (GeneralPrefix + "On");
    public static LocalizedString Off = new (GeneralPrefix + "Off");
    public static LocalizedString TechnicalStatus = new (GeneralPrefix + "Technical status");
    public static LocalizedString Operational = new LocalizedString(GeneralPrefix + "Operational");
    public static LocalizedString Damaged = new LocalizedString(GeneralPrefix + "Damaged");
    public static LocalizedString Critical = new LocalizedString(GeneralPrefix + "Critical");
    public static LocalizedString Broken = new LocalizedString(GeneralPrefix + "Broken");
    public static LocalizedString PossibleUpgrades = new LocalizedString(GeneralPrefix + "Possible upgrades");
    
    #endregion

    #region Parameters Keys
    static public string ItemKey = "ITEM";
    static public string LocationKey = "LOCATION";
    static public string ValueKey = "VALUE";
    static public string Value2Key = "VALUE2";
    static public string Value3Key = "VALUE3";
    #endregion

    
    public static LocalizedString GetLocalizedString(string key)
    {
        LocalizedString localizedString = new LocalizedString(key);

        if (string.IsNullOrEmpty(localizedString))
            return "Incorrect key";

        return localizedString;
    }

    public static LocalizedString GetTranslation(string prefix, string key)
    {
        LocalizedString localizedString = new LocalizedString($"{prefix}{key}");

        if (string.IsNullOrEmpty(localizedString))
            return "Incorrect key";

        return localizedString;
    }

    public static string Translate(this string translationKey, Dictionary<string, object> parameters = null)
    {
        string temp;
        if (!LocalizationManager.TryGetTranslation(translationKey, out temp)) temp = translationKey;
        if (parameters != null && parameters.Count > 0) LocalizationManager.ApplyLocalizationParams(ref temp, parameters);
        return temp;
    }

    public static string Translate(string translationKey, params TranslationParameter[] parameters)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        foreach (TranslationParameter parameter in parameters)
        {
            dict.Add(parameter.Key, parameter.Value);
        }

        return Translate(translationKey, dict);
    }

    public static string Translate(string translationKey, TranslationParameter parameter)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add(parameter.Key, parameter.Value);

        return Translate(translationKey, dict);
    }
}

public class TranslationParameter
{
    public string Key = "";
    public string Value = "";

    public TranslationParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public TranslationParameter(string key, int value)
    {
        Key = key;
        Value = value.ToString();
    }

    public TranslationParameter(string key, float value)
    {
        Key = key;
        Value = $"{value:F0}";
    }

    public TranslationParameter(TranslationParameter translationParameter)
    {
        Key = translationParameter.Key;
        Value = translationParameter.Value;
    }

    public void TranslateValue(params TranslationParameter[] parameters)
    {
        Value = TranslationKeys.Translate(Value, parameters);
    }
}
