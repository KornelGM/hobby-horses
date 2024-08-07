using Cinemachine;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class HobbyHorseMovement : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }
    public float SetSpeed => _setSpeed;
    public float Velocity => _velocity;
    public CharacterController CharacterController => _characterController;
    public CustomHobbyHorse CustomHobbyHorse => _customHobbyHorse;

    public Action<float> OnActualSpeedChange;
    public Action<float> OnVelocityChange;
    public Action OnLanding;

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
    [SerializeField, FoldoutGroup("Constant values")] private float _maxDifferenceValocitySetSpeed;
    [SerializeField, FoldoutGroup("Constant values")] private float _minOnAirSpeed;
    [SerializeField, FoldoutGroup("Constant values")] private float _rotateAccelerateOnAirDivider;

    [SerializeField, FoldoutGroup("Curves")] private AnimationCurve _positionCurve;
    [SerializeField, FoldoutGroup("Curves")] private AnimationCurve _cameraTiltCurve;

    [SerializeField, FoldoutGroup("References")] private Transform _target;
    [SerializeField, FoldoutGroup("References")] private Animator _animator;
    [SerializeField, FoldoutGroup("References")] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField, FoldoutGroup("References")] private CustomHobbyHorse _customHobbyHorse;

    private float _setSpeed;
    private float _velocity;
    private float _setRotateSpeed;
    private Vector3 _oldPosition;
    private bool _isGrounded;

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
        if (isGrounded && !_isGrounded)
            OnLanding?.Invoke();

        _isGrounded = isGrounded;

        CalculateSpeed(moveInput.z, _isGrounded);
        CalculateRotateSpeed(moveInput.x, _isGrounded);

        // ----------------- Influence of turns
        _animator.SetLayerWeight(turnIndex, (Mathf.Abs(_setRotateSpeed)) / 3);
        _animator.SetFloat("Twist", Mathf.InverseLerp(-1, _maxRotateSpeed, _setRotateSpeed));
    }

    public void Move()
    {
        if (!_characterController.enabled)
            return;

        _animator.SetBool("Jump", !_isGrounded);
        _animator.SetFloat("Speed", Mathf.InverseLerp(0, _maxSpeed, _velocity));

        MoveForward(_setSpeed);   
    }

    public void Rotate()
    {
        RotatePlayer();
    }

    protected float CalculateSpeed(float verticalInput, bool isGrounded)
    {
        if(!isGrounded)
        {
            _setSpeed -= _dragOnAir * (_gravityController.CurrGravity * 0.5f) * Time.deltaTime;
            _setSpeed = Mathf.Clamp(_setSpeed, _minOnAirSpeed, 100);
            OnActualSpeedChange?.Invoke(_velocity);
            return _setSpeed;
        }

        if (verticalInput == 0)
        {
            _setSpeed -= _drag * Time.deltaTime;
            _setSpeed = Mathf.Clamp(_setSpeed, 0, _maxSpeed);
        }
        else if (_setSpeed - _velocity <= _maxDifferenceValocitySetSpeed)
        {
            float accelerate = verticalInput > 0 
                ? verticalInput * _accelerate 
                : verticalInput * _brakForce;

            _setSpeed += accelerate * Time.deltaTime;
            _setSpeed = Mathf.Clamp(_setSpeed, _maxBackwardSpeed, _maxSpeed);
        }
        else
        {
            _setSpeed = _velocity;
        }

        OnActualSpeedChange?.Invoke(_setSpeed);
        return _setSpeed;
    }

    protected float CalculateRotateSpeed(float horizontalInput, bool isGrounded)
    {
        if (_setRotateSpeed < 0 && horizontalInput >= 0)
        {
            _setRotateSpeed += (_rotateDrag) * Time.deltaTime;
            _setRotateSpeed = Mathf.Clamp(_setRotateSpeed, -_maxRotateSpeed, 0);
        }
        else if (_setRotateSpeed > 0 && horizontalInput <= 0)
        {
            _setRotateSpeed -= (_rotateDrag) * Time.deltaTime;
            _setRotateSpeed = Mathf.Clamp(_setRotateSpeed, 0, _maxRotateSpeed);
        }

        if (horizontalInput != 0)
        {
            float accelerate = isGrounded 
                ? horizontalInput * _rotateAccelerate
                : horizontalInput * (_rotateAccelerate / _rotateAccelerateOnAirDivider);

            _setRotateSpeed += accelerate * Time.deltaTime;
            _setRotateSpeed = Mathf.Clamp(_setRotateSpeed, -_maxRotateSpeed, _maxRotateSpeed);
        }

        return _setRotateSpeed;
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
        var frameRotationSpeed = _movementSettings.RotationSpeed * _setRotateSpeed;

        //if (ReInput.controllers.GetLastActiveController().type == ControllerType.Joystick)
        //    frameRotationSpeed = _movementSettings.GamepadRotationSpeedMultiplier * _setSpeed;

        Quaternion targetAngle = _characterController.transform.rotation * Quaternion.Euler(0, frameRotationSpeed, 0);

        float targetPosition = Mathf.InverseLerp(-_maxRotateSpeed, _maxRotateSpeed, _setRotateSpeed);
        float evaluateTargetPosition = _positionCurve.Evaluate(targetPosition);
        float evaluateCameraTilt = _cameraTiltCurve.Evaluate(targetPosition);

        _virtualCamera.m_Lens.Dutch = evaluateCameraTilt;
        _target.localPosition = new Vector3(evaluateTargetPosition, _target.localPosition.y, _target.localPosition.z);

        _characterController.transform.rotation = targetAngle;
    }
}
