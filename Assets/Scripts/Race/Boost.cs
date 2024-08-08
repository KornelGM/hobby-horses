using UnityEngine;

public abstract class Boost : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] protected HobbyHorsePlayerManager _playerManager;

    protected virtual void Start()
    {
        InitializeBoost();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        ActivateBoost();
    }

    public abstract void InitializeBoost();
    public abstract void ActivateBoost();

}
