using Rewired;
using UnityEngine;


public class FPSCharacterRotator : MonoBehaviour, ICharacterRotator, IServiceLocatorComponent, IAwake
{
    [SerializeField] private MovementSettings _movementSettings;

    public ServiceLocator MyServiceLocator { get; set; }
    private CharacterController _characterController;

    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
        _movementSettings.IsNotNull(this, nameof(_movementSettings));
    }

    public void Rotate(float MouseX)
    {
        RotatePlayer(MouseX);
    }
    public void RotateTo(Quaternion rotation) => RotateTo(rotation.eulerAngles.y);
    public void RotateTo(float AngleZ)
    {
        Quaternion targetAngle = Quaternion.Euler(0, AngleZ, 0);
        _characterController.transform.rotation = targetAngle;
    }

    private void RotatePlayer(float MouseX)
    {
        var frameRotationSpeed = _movementSettings.RotationSpeed;

        if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
            frameRotationSpeed = _movementSettings.GamepadRotationSpeedMultiplier;

        Quaternion targetAngle = _characterController.transform.rotation * Quaternion.Euler(0, MouseX * frameRotationSpeed, 0);
        _characterController.transform.rotation = targetAngle;
    }

}
