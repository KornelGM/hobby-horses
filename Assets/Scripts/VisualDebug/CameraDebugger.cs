using System;
using UnityEngine;

public class CameraDebugger : VisualDebugger
{
    public static event Action ToggleUI;

    [ServiceLocatorComponent] protected CameraManager _cameraManager;
    private bool _freeCameraEnabled = false;

    private void Start()
    {
        base.CustomStart();

        AddButton(this, b => ToggleCamera(), buttonName: "Toggle Free Camera", color: Color.green);
        AddButton(this, b => ToggleOrbit(), buttonName: "Toggle Orbit", color: Color.yellow);
        AddButton(this, b => ToggleVisibility(), buttonName: "Toggle Sphere Visual", color: Color.yellow);
        AddButton(this, b => ToggleReverse(), buttonName: "Toggle Reverse", color: Color.yellow);
        AddButtonForMethod(this, nameof(SetupRotationSpeed), color: Color.yellow);
        AddButtonForMethod(this, nameof(SetupRadius), color: Color.yellow);
        AddButtonForMethod(this, nameof(SetupMaxAngle), color: Color.yellow);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadDivide) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightAlt))
        {
            ToggleUI?.Invoke();
        }
    }

    private void ToggleCamera()
    {
        if(_freeCameraEnabled)
        {
            _freeCameraEnabled = false;
            ChangeToDefaultCamera();
        }
        else
        {
            _freeCameraEnabled = true;
            ChangeToFreeCamera();
        }
    }

    private void ToggleVisibility()
    {
        _cameraManager.RotateAround.ToggleVisibility();
    }

    private void ToggleReverse()
    {
        _cameraManager.RotateAround.ToggleReverse();
    }

    private void ToggleOrbit()
    {
        _cameraManager.RotateAround.Toggle();
    }

    public void SetupRotationSpeed(float speed)
    {
        _cameraManager.RotateAround.SetupSpeed(speed);
    }

    public void SetupRadius(float radius)
    {
        _cameraManager.RotateAround.SetupRadius(radius);
    }

    public void SetupMaxAngle(float maxAngle)
    {
        _cameraManager.RotateAround.SetupMaxAngle(maxAngle);
    }

    private void ChangeToFreeCamera()
    {
        _cameraManager.ChangeToFreeCamera();
    }

    private void ChangeToDefaultCamera()
    {
        _cameraManager.ChangeToDefaultCamera();
    }
}
