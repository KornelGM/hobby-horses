using UnityEngine;

public class TestComponent : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    [ServiceLocatorComponent] private TestManager _testManager;

    public ServiceLocator MyServiceLocator { get; set; }

    public void CustomStart()
    {
        _testManager.Scream(gameObject);
    }
}
