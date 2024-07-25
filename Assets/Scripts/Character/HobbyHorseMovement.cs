using Rewired;
using System;
using UnityEngine;

public class HobbyHorseMovement : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public Action<float> OnActualSpeedChange;

    [SerializeField] private MovementSettings _movementSettings;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxRotateSpeed;
    [SerializeField] private float _brakForce;
    [SerializeField] private float _accelerate;
    [SerializeField] private float _rotateAccelerate;
    [SerializeField] private float _rotateAccelerateOnAir;
    [SerializeField] private float _drag;
    [SerializeField] private float _dragOnAir;
    [SerializeField] private float _rotateDrag;
    [SerializeField] private Transform _target;
    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private Animator _animator;

    public CharacterController CharacterController => _characterController;
    public ServiceLocator MyServiceLocator { get; set; }
    public float ActualSpeed => _actualSpeed;
    private float _actualSpeed;
    private float _actualRotateSpeed;
    private CharacterController _characterController;
    private float _lastActualSpeed;

    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
        _movementSettings.IsNotNull(this, nameof(_movementSettings));
    }

    public void Move(Vector3 moveInput, bool isGrounded)
    {
        if (!_characterController.enabled)
            return;

        _animator.SetFloat("Speed", Mathf.InverseLerp(0, _maxSpeed, _actualSpeed));

        CalculateRotateSpeed(moveInput.x, isGrounded);
        RotatePlayer(moveInput);

        CalculateSpeed(moveInput.z, isGrounded);
        MoveForward(_actualSpeed);
    }

    protected float CalculateSpeed(float verticalInput, bool isGrounded)
    {
        if(!isGrounded)
        {
            _actualSpeed -= _dragOnAir * Time.deltaTime;
            _actualSpeed = Mathf.Clamp(_actualSpeed, 0, _maxSpeed);
            OnActualSpeedChange?.Invoke(_actualSpeed);
            return _actualSpeed;
        }

        if (verticalInput == 0)
        {
            _actualSpeed -= _drag * Time.deltaTime;
            _actualSpeed = Mathf.Clamp(_actualSpeed, 0, _maxSpeed);
        }
        else
        {
            float accelerate = verticalInput > 0 
                ? verticalInput * _accelerate 
                : verticalInput * _brakForce;

            _actualSpeed += accelerate * Time.deltaTime;
            _actualSpeed = Mathf.Clamp(_actualSpeed, -1, _maxSpeed);
        }

        OnActualSpeedChange?.Invoke(_actualSpeed);
        return _actualSpeed;
    }

    protected float CalculateRotateSpeed(float horizontalInput, bool isGrounded)
    {
        if (_actualRotateSpeed < 0 && horizontalInput >= 0)
        {
            _actualRotateSpeed += (_rotateDrag) * Time.deltaTime;
            _actualRotateSpeed = Mathf.Clamp(_actualRotateSpeed, -_maxRotateSpeed, 0);
        }
        else if (_actualRotateSpeed > 0 && horizontalInput <= 0)
        {
            _actualRotateSpeed -= (_rotateDrag) * Time.deltaTime;
            _actualRotateSpeed = Mathf.Clamp(_actualRotateSpeed, 0, _maxRotateSpeed);
        }

        if (horizontalInput != 0)
        {
            float accelerate = isGrounded 
                ? horizontalInput * _rotateAccelerate
                : horizontalInput * _rotateAccelerateOnAir;

            _actualRotateSpeed += accelerate * Time.deltaTime;
            _actualRotateSpeed = Mathf.Clamp(_actualRotateSpeed, -_maxRotateSpeed, _maxRotateSpeed);
        }

        return _actualRotateSpeed;
    }

    protected void MoveForward(float speed)
    {
        _characterController.Move(transform.forward * speed * Time.deltaTime);
    }

    private void RotatePlayer(Vector3 input)
    {
        var frameRotationSpeed = _movementSettings.RotationSpeed * _actualRotateSpeed;

        if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
            frameRotationSpeed = _movementSettings.GamepadRotationSpeedMultiplier * _actualSpeed;

        Quaternion targetAngle = _characterController.transform.rotation * Quaternion.Euler(0, frameRotationSpeed, 0);

        float targetPosition = Mathf.InverseLerp(-_maxRotateSpeed, _maxRotateSpeed, _actualRotateSpeed);
        float evaluateTargetPosition = _positionCurve.Evaluate(targetPosition);

        _target.localPosition = new Vector3(evaluateTargetPosition, _target.localPosition.y, _target.localPosition.z);

        _characterController.transform.rotation = targetAngle;
    }
}
