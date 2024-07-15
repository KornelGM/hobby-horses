public interface IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public void SetupServiceLocator(ServiceLocator parent)
    {
        if (MyServiceLocator != null) return;
        MyServiceLocator = parent;
    }
}
