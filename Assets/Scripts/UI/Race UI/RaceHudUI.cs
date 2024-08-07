using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceHudUI : MonoBehaviour, IWindow, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; }

    [ServiceLocatorComponent] private HobbyHorsePlayerManager _playerManager;
    [ServiceLocatorComponent] private SlowMotionManager _slowMotionManager;

    [SerializeField] private GameObject _helpTTPanel;
    [SerializeField] private TextMeshProUGUI _speedValueText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private TextMeshProUGUI _startTimerText;
    [SerializeField] private GameObject _jumpBar;
    [SerializeField] private float _jumpBarVisibilityTreshold;
    [SerializeField] private Image _jumpForceBar;

    [SerializeField] private GameObject _slowMotionBar;
    [SerializeField] private float _slowMotionVisibilityTreshold;
    [SerializeField] private Image _slowMotionTimeBar;

    private HobbyHorseMovement _movement;
    private GravityCharacterController _gravityController;

    private float _time;
    private bool _timer;
    private Coroutine _coroutine;

    public void Initialize()
    {
        _startPanel.SetActive(false);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _movement);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _gravityController);

        _movement.OnVelocityChange += UpdateSpeedValue;
        _gravityController.OnJumpForceChange += UpdateJumpForce;
        _slowMotionManager.OnSlowMotionTimeChange += UpdateSlowMotionBar;

        UpdateJumpForce(0);
        UpdateSpeedValue(0);
        UpdateSlowMotionBar(_slowMotionManager.MaxSlowMotionTime);
    }

    private void UpdateSpeedValue(float speed)
    {
        _speedValueText.text = $"{speed:F1}";
    }

    private void UpdateJumpForce(float force)
    {
        float evaluatedJumpForce = Mathf.InverseLerp(_gravityController.MinJumpForce, _gravityController.MaxJumpForce, force);
        _jumpBar.SetActive(evaluatedJumpForce > _jumpBarVisibilityTreshold);
        _jumpForceBar.fillAmount = evaluatedJumpForce;
    }

    private void UpdateSlowMotionBar(float time)
    {
        float evaluatedSlowMotionTime = Mathf.InverseLerp(0, _slowMotionManager.MaxSlowMotionTime, time);
        _slowMotionBar.SetActive(evaluatedSlowMotionTime < _slowMotionVisibilityTreshold);
        _slowMotionTimeBar.fillAmount = evaluatedSlowMotionTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(StartTimer());
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _helpTTPanel.SetActive(!_helpTTPanel.activeSelf);
        }

        if (_timer)
        {
            _time += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (_timer)
        {
            UpdateTimer(_time);
        }
    }

    private void UpdateTimer(float time)
    {
        var ts = System.TimeSpan.FromSeconds(time);
        _timerText.text = ts.ToString("mm\\:ss\\:ff");
    }

    private IEnumerator StartTimer()
    {
        _startPanel.SetActive(true);
        _timer = false;
        _time = 0;
        UpdateTimer(_time);

        _startTimerText.text = "";
        yield return new WaitForSecondsRealtime(1f);
        _startTimerText.text = $"3";
        yield return new WaitForSecondsRealtime(1f);
        _startTimerText.text = $"2";
        yield return new WaitForSecondsRealtime(1f);
        _startTimerText.text = $"1";
        yield return new WaitForSecondsRealtime(1f);
        _startTimerText.text = $"GO!";

        _timer = true;

        yield return new WaitForSecondsRealtime(1f);
        _startTimerText.text = "";
        _startPanel.SetActive(false);
    }
}
