
public class AutoInjectingServiceLocator : IServiceLocatorComponent, IAwake, ITestComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private ITestComponent _component;

    public void CustomAwake()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out _component);
    }
}
