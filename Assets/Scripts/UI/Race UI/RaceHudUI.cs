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

    [SerializeField] private TextMeshProUGUI _speedValueText;
    [SerializeField] private GameObject _jumpBar;
    [SerializeField] private float _jumpBarVisibilityTreshold;
    [SerializeField] private Image _jumpForceBar;

    private HobbyHorseMovement _movement;
    private GravityCharacterController _gravityController;

    public void Initialize()
    {
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _movement);
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _gravityController);

        _movement.OnActualSpeedChange += UpdateSpeedValue;
        _gravityController.OnJumpForceChange += UpdateJumpForce;
    }

    private void UpdateSpeedValue(float speed)
    {
        _speedValueText.text = $"{speed:F1}";
    }

    private void UpdateJumpForce(float force)
    {
        float evaluatedJumpForce = Mathf.InverseLerp(0, _gravityController.MaxJumpForce, force);
        _jumpBar.SetActive(evaluatedJumpForce > _jumpBarVisibilityTreshold);
        _jumpForceBar.fillAmount = evaluatedJumpForce;
    }
}
