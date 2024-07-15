using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Clock : MonoBehaviour, IAwake, IServiceLocatorComponent
{
    [Header("Time Scale")]
    [Range(0,100)] public float TimeScale;
    [SerializeField] private Slider timeScaleSlider;
    private float minValueTimeScale = 0f;
    private float maxValueTimeScale = 100f;

    [Header("Time Change")]
    [Range(0,WorldTimeManager.HoursInDay)] public float TimeChange;
    [SerializeField] private Slider timeChangeSlider;
    private float minValueTimeChange = 0f;
    private float maxValueTimeChange = WorldTimeManager.HoursInDay;

    [Header("Other")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private GameObject _sliders;

    [Header("Managers")]
    [ServiceLocatorComponent] private WorldTimeManager _worldTimeManager;
    [SerializeField] private CursorManager _cursorManager;

    public ServiceLocator MyServiceLocator { get; set; }

    private bool _examplePanelEnabled = false;

    public void CustomAwake()
    {
        _worldTimeManager.IsNotNull(this, nameof(_worldTimeManager));
        _cursorManager.IsNotNull(this, nameof(_cursorManager));
        TimeScale = 1f;
    }

    void Start()
    {
        StartTime();
        SetUpTimeScale();
        SetUpTimeChange();
    }
    
    void Update()
    {
        ToggleCursorVisibility();
        DisplayTime();
        _dayText.text = "Day: " + _worldTimeManager.Days;
        timeScaleSlider.value = TimeScale;
        timeChangeSlider.value = TimeChange;
    }
    
    public float GetValueTimeScale()
    {
        return TimeScale;
    }

    private void UpdateValueTimeScale(float value)
    {
        TimeScale = value;
    }

    private void SetUpTimeScale()
    {
        timeScaleSlider.minValue = minValueTimeScale;
        timeScaleSlider.maxValue = maxValueTimeScale;
        timeScaleSlider.value = TimeScale;
        timeScaleSlider.onValueChanged.AddListener(UpdateValueTimeScale);
    }

    public float GetValueTimeChange()
    {
        return TimeChange;
    }

    private void UpdateValueTimeChange(float value)
    {
        TimeChange = value;
    }

    private void SetUpTimeChange()
    {
        timeChangeSlider.minValue = minValueTimeChange;
        timeChangeSlider.maxValue = maxValueTimeChange;
        timeChangeSlider.value = TimeChange;
        timeChangeSlider.onValueChanged.AddListener(UpdateValueTimeChange);
    }

    private void StartTime()
    {
        _worldTimeManager.Days = 1;
        _worldTimeManager.Time = TimeChange;
    }

    private void DisplayTime()
    {
        _timerText.text = string.Format("{0:00}:{1:00}", _worldTimeManager.Hour, _worldTimeManager.Minutes);
    }

    private void ToggleCursorVisibility()
    {
        if (!Input.GetKeyDown(KeyCode.Z))
            return;

        if (!_examplePanelEnabled)
        {
            _sliders.SetActive(true);
            _cursorManager.ActivateCursor();
        }
        else
        {
            _sliders.SetActive(false);
            _cursorManager.DeactivateCursor();
        }

        _examplePanelEnabled = !_examplePanelEnabled;
    }
    
}
