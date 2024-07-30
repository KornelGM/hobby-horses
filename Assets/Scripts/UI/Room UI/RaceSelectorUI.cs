using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceSelectorUI : MonoBehaviour, IServiceLocatorComponent, IWindow
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; }

    [ServiceLocatorComponent] private RaceSelectorManager _raceSelectorManager;

    [SerializeField, FoldoutGroup("References")] private TextMeshProUGUI _raceName;
    [SerializeField, FoldoutGroup("References")] private Image _raceIcon;
    [SerializeField, FoldoutGroup("References")] private LoadingWindow _loadingWindow;
    [SerializeField, FoldoutGroup("References")] private RoomPartButton _backButton;

    private RaceInfo _currentRace;
    private int _raceIndex = 0;

    public void Initialize()
    {
        _currentRace = _raceSelectorManager.RaceInfo[_raceIndex];
        UpdateVisuals();
    }

    public void SwitchRace(bool positive)
    {
        if(positive)
            _raceIndex++;
        else
            _raceIndex--;

        if (_raceIndex >= _raceSelectorManager.RaceInfo.Length)
            _raceIndex = 0;
        else if (_raceIndex < 0)
            _raceIndex = _raceSelectorManager.RaceInfo.Length - 1;

        _currentRace = _raceSelectorManager.RaceInfo[_raceIndex];
        UpdateVisuals();
    }

    public void StartRace()
    {
        _raceSelectorManager.StartRace(_currentRace, _loadingWindow);
    }

    public void StopSelecting()
    {
        _raceSelectorManager.StopSelecting(_backButton.OnButtonDown);
    }

    private void UpdateVisuals()
    {
        _raceName.text = _currentRace.RaceName;
        _raceIcon.sprite = _currentRace.RaceIcon;
    }
}
