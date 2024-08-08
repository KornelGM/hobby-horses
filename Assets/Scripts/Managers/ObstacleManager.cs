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
            guide.InitializeTrackGuide();
            guide.SetGuidesDeactivated();
        }

        _obstacles[0].enabled = true;
        _guides[0].SetGuidesActive();
        _guides[1].SetGuidesActive(true);
    }

    public void TurnOutline(int index)
    {
        if (index - 1 != _index)
            return;

        _obstacles[index].enabled = false;
        _guides[index].SetGuidesDeactivated();
        _index = index;

        if (index + 1 >= _obstacles.Length)
        {
            _index = -1;
            _obstacles[0].enabled = true;
            _guides[0].SetGuidesActive();
            _guides[1].SetGuidesActive(true);
            return;
        }

        _obstacles[index + 1].enabled = true;
        _guides[index + 1].SetGuidesActive();

        if (index + 2 < _obstacles.Length)
            _guides[index + 2].SetGuidesActive(true);
    }
}
