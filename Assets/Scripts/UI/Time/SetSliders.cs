using UnityEngine.UI;
using UnityEngine;

public class SetSliders : MonoBehaviour, IAwake, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private Clock _clock;

    [Header("Sliders")]
    [SerializeField] private Slider _timeScaleSlider;
    [SerializeField] private Slider _timeChangeSlider;

    [ServiceLocatorComponent] private WorldTimeManager _worldTimeManager;

    public void CustomAwake()
    {
        _worldTimeManager.IsNotNull(this, nameof(_worldTimeManager));
        _clock.IsNotNull(this, nameof(_clock));
    }

    void Start()
    {
        SetupTimeScaleSlider();
        SetupTimeSlider();
    }

    void Update()
    {
        _timeChangeSlider.value = _worldTimeManager.Time;
    }

    private void SetupTimeSlider() 
    {
        _timeChangeSlider.minValue = 0f;
        _timeChangeSlider.maxValue = WorldTimeManager.HoursInDay;
        _timeChangeSlider.value = _clock.TimeChange;
        _timeChangeSlider.onValueChanged.AddListener(OnTimeChanged);
    }

    private void SetupTimeScaleSlider() 
    {
        _timeScaleSlider.minValue = 0f;
        _timeScaleSlider.maxValue = 10;
        _timeScaleSlider.value = 1f;
        _timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
    }

    public void OnTimeScaleChanged(float value)
    {
        _worldTimeManager.TimeScale = value;
    }

    private void OnTimeChanged(float value)
    {
        _worldTimeManager.Time = value;
    }

    public void ResetTimeScaleButton()
    {
        _timeScaleSlider.value = 1f;
    }
    
}
