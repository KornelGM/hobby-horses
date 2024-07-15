using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotator : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }
    public Transform Camera => _camera;

    [SerializeField] private MovementSettings _movementSettings;

    private Transform _camera;

    public void CustomAwake()
    {
        if(MyServiceLocator.TryGetServiceLocatorComponent(out PlayerStateMachine stateMachine))
        {
            _camera = stateMachine.FirstPersonCamera;
        }
        _camera.IsNotNull(this, nameof(_camera));
    }

    /// <summary>
    /// Call in update
    /// </summary>
    /// <param name="MouseY"></param>
    public void Rotate(float MouseY) => RotateCamera(MouseY);
    public void RotateTo(Quaternion rotation)
    {
        float angle = rotation.eulerAngles.x;
        if (angle > 180) angle -= 360;
        angle = Mathf.Clamp(angle, -80, 80);
        _camera.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    private void RotateCamera(float MouseY)
    {
        Quaternion rotation = _camera.localRotation * Quaternion.Euler(-MouseY * _movementSettings.RotationSpeed, 0, 0);
        if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
        {
            rotation = _camera.localRotation * Quaternion.Euler(-MouseY * _movementSettings.GamepadRotationSpeedMultiplier, 0, 0);
        }

        RotateTo(rotation);
    }
}
