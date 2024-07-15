
public class ChangeRadioStation : BaseAction, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private Radio _radio;

    public override bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand)
    {
        _radio.ChangeRadioStation();
    }
}
