using Cinemachine;
using Rewired;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class HobbyHorseMovement : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }
    public float ActualSpeed => _actualSpeed;
    public float Velocity => _velocity;
    public CharacterController CharacterController => _characterController;
    public CustomHobbyHorse CustomHobbyHorse => _customHobbyHorse;

    public Action<float> OnActualSpeedChange;
    public Action<float> OnVelocityChange;

    [ServiceLocatorComponent] private GravityCharacterController _gravityController;

    [SerializeField, FoldoutGroup("Global Settings")] private MovementSettings _movementSettings;

    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _maxSpeed;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _maxRotateSpeed;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _brakForce;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _accelerate;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _rotateAccelerate;

    [SerializeField, FoldoutGroup("Constant values")] private float _drag;
    [SerializeField, FoldoutGroup("Constant values")] private float _dragOnAir;
    [SerializeField, FoldoutGroup("Constant values")] private float _rotateDrag;
    [SerializeField, FoldoutGroup("Constant values")] private float _maxBackwardSpeed;
    [SerializeField, FoldoutGroup("Constant values")] private float _minOnAirSpeed;
    [SerializeField, FoldoutGroup("Constant values")] private float _rotateAccelerateOnAirDivider;

    [SerializeField, FoldoutGroup("Curves")] private AnimationCurve _positionCurve;
    [SerializeField, FoldoutGroup("Curves")] private AnimationCurve _cameraTiltCurve;

    [SerializeField, FoldoutGroup("References")] private Transform _target;
    [SerializeField, FoldoutGroup("References")] private Animator _animator;
    [SerializeField, FoldoutGroup("References")] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField, FoldoutGroup("References")] private CustomHobbyHorse _customHobbyHorse;

    private float _actualSpeed;
    private float _velocity;
    private float _actualRotateSpeed;
    private Vector3 _oldPosition;

    // ---------------------------- Controller
    private CharacterController _characterController;
    private int turnIndex;
    private int jumpIndex;

    // ----------------------------- Falling


    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
        _movementSettings.IsNotNull(this, nameof(_movementSettings));
        _oldPosition = _characterController.transform.position;

        // ----------------------------------------- Animation Controller
        turnIndex = _animator.GetLayerIndex("Turn");
        jumpIndex = _animator.GetLayerIndex("Jump");
    }

    public void SetStats(HobbyHorseStats stats)
    {
        _maxSpeed = stats.MaxSpeed;
        _maxRotateSpeed = stats.MaxRotateSpeed;
        _brakForce = stats.BrakeForce;
        _accelerate = stats.Accelerate;
        _rotateAccelerate= stats.RotateAccelerate;
    }

    public void SetStats(HobbyHorseStats[] hobbyHorseStats)
    {
        if (hobbyHorseStats is not { Length: > 0 })
            return;

        float maxSpeed = 0;
        float maxRotateSpeed = 0;
        float brakeForce = 0;
        float accelerate = 0;
        float rotateAccelerate = 0;

        foreach (var stats in hobbyHorseStats)
        {
            maxSpeed += stats.MaxSpeed;
            maxRotateSpeed += stats.MaxRotateSpeed;
            brakeForce += stats.BrakeForce;
            accelerate += stats.Accelerate;
            rotateAccelerate += stats.RotateAccelerate;
        }

        _maxSpeed = maxSpeed;
        _maxRotateSpeed = maxRotateSpeed;
        _brakForce = brakeForce;
        _accelerate = accelerate;
        _rotateAccelerate = rotateAccelerate;   
    }

    public void CalculateInput(Vector3 moveInput, bool isGrounded)
    {
        CalculateSpeed(moveInput.z, isGrounded);
        CalculateRotateSpeed(moveInput.x, isGrounded);

        // ----------------- Influence of turns
        _animator.SetLayerWeight(turnIndex, (Mathf.Abs(_actualRotateSpeed)) / 3);
        _animator.SetFloat("Twist", Mathf.InverseLerp(-1, _maxRotateSpeed, _actualRotateSpeed));
    }

    public void Move(bool isGrounded)
    {
        if (!_characterController.enabled)
            return;

        _animator.SetBool("Jump", !isGrounded);
        _animator.SetFloat("Speed", Mathf.InverseLerp(0, _maxSpeed, _velocity));


        MoveForward(_actualSpeed);   
    }

    public void Rotate()
    {
        RotatePlayer();
    }

    protected float CalculateSpeed(float verticalInput, bool isGrounded)
    {
        if(!isGrounded)
        {
            _actualSpeed -= _dragOnAir * (_gravityController.CurrGravity * 0.5f) * Time.deltaTime;
            _actualSpeed = Mathf.Clamp(_actualSpeed, _minOnAirSpeed, 100);
            OnActualSpeedChange?.Invoke(_velocity);
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
            _actualSpeed = Mathf.Clamp(_actualSpeed, _maxBackwardSpeed, _maxSpeed);
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
                : horizontalInput * (_rotateAccelerate / _rotateAccelerateOnAirDivider);

            _actualRotateSpeed += accelerate * Time.deltaTime;
            _actualRotateSpeed = Mathf.Clamp(_actualRotateSpeed, -_maxRotateSpeed, _maxRotateSpeed);
        }

        return _actualRotateSpeed;
    }

    protected void MoveForward(float speed)
    {
        _characterController.Move(transform.forward * speed * Time.fixedDeltaTime);
        CalculateVelocity();
    }


    private void CalculateVelocity()
    {
        float velocity = _velocity;
        _velocity = Vector3.Distance(_oldPosition, _characterController.transform.position) / Time.fixedDeltaTime;

        if (_velocity != velocity)
            OnVelocityChange?.Invoke(_velocity);

        _oldPosition = _characterController.transform.position;
    }

    private void RotatePlayer()
    {
        var frameRotationSpeed = _movementSettings.RotationSpeed * _actualRotateSpeed;

        if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
            frameRotationSpeed = _movementSettings.GamepadRotationSpeedMultiplier * _actualSpeed;

        Quaternion targetAngle = _characterController.transform.rotation * Quaternion.Euler(0, frameRotationSpeed, 0);

        float targetPosition = Mathf.InverseLerp(-_maxRotateSpeed, _maxRotateSpeed, _actualRotateSpeed);
        float evaluateTargetPosition = _positionCurve.Evaluate(targetPosition);
        float evaluateCameraTilt = _cameraTiltCurve.Evaluate(targetPosition);

        _virtualCamera.m_Lens.Dutch = evaluateCameraTilt;
        _target.localPosition = new Vector3(evaluateTargetPosition, _target.localPosition.y, _target.localPosition.z);

        _characterController.transform.rotation = targetAngle;
    }
}
