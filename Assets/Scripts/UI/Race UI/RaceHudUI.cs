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

    [SerializeField] private TextMeshProUGUI _speedValueText;
    [SerializeField] private GameObject _jumpBar;
    [SerializeField] private float _jumpBarVisibilityTreshold;
    [SerializeField] private Image _jumpForceBar;

    [SerializeField] private GameObject _slowMotionBar;
    [SerializeField] private float _slowMotionVisibilityTreshold;
    [SerializeField] private Image _slowMotionTimeBar;

    private HobbyHorseMovement _movement;
    private GravityCharacterController _gravityController;

    public void Initialize()
    {
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
}
