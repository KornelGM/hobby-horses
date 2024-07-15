using I2.Loc;

public class LocalizationDebugConsole : VisualDebugger, IServiceLocatorComponent
{
    public override void CustomStart()
    {
        base.CustomStart();
        foreach (string language in LocalizationManager.GetAllLanguages())
        {
            AddButton(this, (button) => ChangeLanguage(language), "", "", language);
        }    
    }

    private void ChangeLanguage(string language)
    {
        LocalizationManager.CurrentLanguage = language;
    }
}
