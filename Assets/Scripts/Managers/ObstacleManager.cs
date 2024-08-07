using EPOOutline;
using UnityEngine;

public class ObstacleManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private Outlinable[] _obstacles;

    private int _index = -1;

    private void Awake()
    {
        foreach (var obstacle in _obstacles)
        {
            obstacle.enabled = false;
        }

        _obstacles[0].enabled = true;
    }

    public void TurnOutline(int index)
    {
        if (index - 1 != _index)
            return;

        _obstacles[index].enabled = false;
        _index = index;

        if (index + 1 >= _obstacles.Length)
        {
            _index = -1;
            _obstacles[0].enabled = true;
            return;
        }

        _obstacles[index + 1].enabled = true;
    }
}
