public interface IAction : IHasTooltip
{
	public int ActionPriority { get; }
	public bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
	public void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand);
	public void StartFinishOfInteraction() { }
	public ClampedValue ProgressBar { get; }
	public void OnStart();
	public void OnStop();
}