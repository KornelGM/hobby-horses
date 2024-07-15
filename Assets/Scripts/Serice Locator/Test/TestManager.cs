using UnityEngine;

public class TestManager : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }


    public void Scream(GameObject component)
    {
        Debug.Log($"{component.name} called manager {gameObject.name}");
    }

    public void CustomAwake()
    {
        Scream(gameObject);
    }
}
