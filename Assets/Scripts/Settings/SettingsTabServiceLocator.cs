public class SettingsTabServiceLocator : ServiceLocator, IAwake, IStartable
{
    public override void CustomAwake()
    {
        Awake();
    }

    public override void CustomStart()
    {
    }
}
