using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour, IMove, IServiceLocatorComponent, IAwake
{
    [SerializeField] private MovementSettings _movementSettings;

    public CharacterController CharacterController => _characterController;
    public ServiceLocator MyServiceLocator { get; set; }
    public float ActualSpeed => _actualSpeed;
    public Vector3 MoveDirection;
    private float _actualSpeed;
    private CharacterController _characterController;
    private float _velocity;
    private float speedMultiplier = 1;

    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
        _movementSettings.IsNotNull(this, nameof(_movementSettings));
        MoveDirection = Vector3.zero;
    }

    /// <summary>
    /// Should be called in update method
    /// </summary>
    /// <param name="controller"></param>
    public void Move(Vector3 moveInput, bool isSprinting)
    {
        if (!_characterController.enabled)
            return;

        MoveDirection = InputToDirection(moveInput);
        Move(MoveDirection * CalculateSpeed(isSprinting));
    }

    public void AddSlowDown(float value)
    {
        speedMultiplier = 1 - value;
    }
    public void RemoveSlowDown()
    {
        speedMultiplier = 1;
    }

    protected float CalculateSpeed(bool isSprinting)
    {
        float targetSpeed;
        if (isSprinting) targetSpeed = _movementSettings.MovementSpeed * _movementSettings.SprintSpeedModification;
        else targetSpeed = _movementSettings.MovementSpeed;

        _actualSpeed = Mathf.SmoothDamp(_actualSpeed, targetSpeed, ref _velocity, 0.3f) * speedMultiplier;
        return _actualSpeed;
    }

    private Vector3 InputToDirection(Vector3 input)
    {
        return _characterController.transform.rotation * input;
    }

    protected void Move(Vector3 velocity)
    {
        _characterController.Move(velocity * Time.deltaTime);
    }
}
