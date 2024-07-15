using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour, IManager, IStartable, IServiceLocatorComponent
{
    public Camera Camera { get; private set; }
    public ServiceLocator MyServiceLocator { get; set; }

    public delegate void CameraEventHandler();
    public static event CameraEventHandler ChangeToFreeCameraMode;
    public static event CameraEventHandler ChangeToDefaultCameraMode;
    public RotateAround RotateAround => _freeCameraRotateAround;

    [ServiceLocatorComponent] private ItemInteractionTooltipManager _tooltipManager;
    public Transform DroneCameraTransform => _droneCamController ? _droneCamController.transform : null;
    [SerializeField] private CinemachineBrain _cmBrain;
    [SerializeField] private DroneCamController _droneCamController;
    [SerializeField] private RotateAround _freeCameraRotateAround;
    private CinemachineVirtualCamera _currentSwitchedCamera = null;
    private int previousPriority = -1;

    public void CustomReset()
    {
        Camera = null;
    }

    public void CustomStart()
    {
        Camera.IsNotNull(this, nameof(Camera));
    }

    public void SetupCamera(Camera camera)
    {
        Camera = camera;
    }

    public void ChangeToFreeCamera(bool stopTime = true)
    {
        _tooltipManager.Hide(this);
        _droneCamController.Enable();
        if (stopTime)
            Time.timeScale = 0;
        _droneCamController?.gameObject.SetActive(true);
        _cmBrain.m_IgnoreTimeScale = true;
        ChangeToFreeCameraMode?.Invoke();
    }

    public void ChangeToDefaultCamera()
    {
        _tooltipManager.Unhide(this);
        _droneCamController.Disable();
        Time.timeScale = 1;
        _droneCamController?.gameObject.SetActive(false);
        _cmBrain.m_IgnoreTimeScale = false;
        ChangeToDefaultCameraMode?.Invoke();
    }

    public void SwitchCamera(CinemachineVirtualCamera other)
    {
        if (other == null) return;
        SwitchBack(); //assure the previous switched camera is handled correctly
        other.gameObject.SetActive(true);
        previousPriority = other.Priority;
        _currentSwitchedCamera = other;
        other.Priority = 100;
    }

    public void SwitchBack()
    {
        if (_currentSwitchedCamera == null) return;
        _currentSwitchedCamera.gameObject.SetActive(false);
        _currentSwitchedCamera.Priority = previousPriority;
        _currentSwitchedCamera = null;
    }
}
