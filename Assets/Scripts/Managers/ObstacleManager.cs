using EPOOutline;
using UnityEngine;

public class ObstacleManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private Outlinable[] _obstacles;
    [SerializeField] private RaceTrackGuide[] _guides;

    private int _index = -1;

    private void Awake()
    {
        foreach (var obstacle in _obstacles)
        {
            obstacle.enabled = false;
        }

        foreach (var guide in _guides)
        {
            guide.gameObject.SetActive(false);
        }

        _obstacles[0].enabled = true;
        _guides[0].gameObject.SetActive(true);
    }

    public void TurnOutline(int index)
    {
        if (index - 1 != _index)
            return;

        _obstacles[index].enabled = false;
        _guides[index].gameObject.SetActive(false);
        _index = index;

        if (index + 1 >= _obstacles.Length)
        {
            _index = -1;
            _obstacles[0].enabled = true;
            _guides[0].gameObject.SetActive(true);
            return;
        }

        _obstacles[index + 1].enabled = true;
        _guides[index + 1].gameObject.SetActive(true);
    }
}
