using System;
using I2.Loc;
using Sirenix.OdinInspector;

[Serializable]
public class UITooltipContent
{
    public bool UseI2 = false;
    [HideIf("UseI2")] public string Title = "";
    [ShowIf("UseI2")][TermsPopup] public string TitleI2 = "";
    [HideIf("UseI2")] public string Description = "";
    [ShowIf("UseI2")][TermsPopup] public string DescriptionI2 = "";
}