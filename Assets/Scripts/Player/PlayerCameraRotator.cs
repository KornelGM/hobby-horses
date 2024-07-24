using Rewired;
using UnityEngine;

public class PlayerCameraRotator : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public bool FreeCamera => _freeCamera;
    public ServiceLocator MyServiceLocator { get; set; }
    public Transform Camera => _camera;

    [SerializeField] private MovementSettings _movementSettings;

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _target;
    [SerializeField] private float _cameraSpeed;

    private bool _freeCamera = false;

    public void CustomAwake()
    {
        _camera.IsNotNull(this, nameof(_camera));
    }

    /// <summary>
    /// Call in update
    /// </summary>
    /// <param name="MouseY"></param>
    public void Rotate(float MouseY, float MouseX) => RotateCamera(MouseY, MouseX);
    public void RotateTo(Quaternion rotation)
    {
        if (!_freeCamera)
            return;

        float angleY = rotation.eulerAngles.x;
        float angleX = rotation.eulerAngles.y;

        if (angleY > 180) angleY -= 360;
        if (angleX > 180) angleX -= 360;

        angleY = Mathf.Clamp(angleY, -20, 40);
        angleX = Mathf.Clamp(angleX, -80, 80);
        _camera.localRotation = Quaternion.Euler(angleY, angleX, 0);
    }

    private void RotateCamera(float MouseY, float MouseX)
    {
        Quaternion rotation = _camera.localRotation * Quaternion.Euler(-MouseY * _movementSettings.RotationSpeed, MouseX * _movementSettings.RotationSpeed, 0);
        if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
        {
            rotation = _camera.localRotation * Quaternion.Euler(-MouseY * _movementSettings.GamepadRotationSpeedMultiplier, 0, 0);
        }

        RotateTo(rotation);
    }

    public void CameraFollowTarget() => CameraFollowTarget(_target);

    private void CameraFollowTarget(Transform target)
    {
        if (_freeCamera)
            return;

        if (target == null)
            return;

        Vector3 direction = target.position - _camera.transform.position;
        Quaternion finalRotation = Quaternion.LookRotation(direction);
        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, finalRotation, _cameraSpeed);
    }

    public void SwitchFreeCamera(bool value)
    {
        _freeCamera = value;
    }

    public void BackToCenterPosition()
    {
        SwitchFreeCamera(false);
    }
}
