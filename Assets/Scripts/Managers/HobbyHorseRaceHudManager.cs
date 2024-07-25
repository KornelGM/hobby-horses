using UnityEngine;

public class HobbyHorseRaceHudManager : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private WindowManager _windowManager;

    [SerializeField] private RaceHudUI _raceHudUI;
    private RaceHudUI _createdRaceHudUI;

    public void CustomStart()
    {
        _createdRaceHudUI =_windowManager.CreateWindow(_raceHudUI).GetComponent<RaceHudUI>();
        _createdRaceHudUI.Initialize();
    }
}
