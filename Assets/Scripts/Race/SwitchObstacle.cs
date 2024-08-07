using UnityEngine;

public class SwitchObstacle : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private int _index;

    [ServiceLocatorComponent] private ObstacleManager _obstacleManager;

    private void OnTriggerEnter(Collider other)
    {
        _obstacleManager.TurnOutline(_index);
    }

}
