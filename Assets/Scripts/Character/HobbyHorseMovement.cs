using Rewired;
using UnityEngine;

public class HobbyHorseMovement : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    [SerializeField] private MovementSettings _movementSettings;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxRotateSpeed;
    [SerializeField] private float _brakForce;
    [SerializeField] private float _accelerate;
    [SerializeField] private float _rotateAccelerate;
    [SerializeField] private float _drag;
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


    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
        _movementSettings.IsNotNull(this, nameof(_movementSettings));
    }

    public void Move(Vector3 moveInput)
    {
        if (!_characterController.enabled)
            return;

        CalculateSpeed(moveInput.z);
        CalculateRotateSpeed(moveInput.x);

        _animator.SetFloat("Speed", Mathf.InverseLerp(0, _maxSpeed, _actualSpeed));

        RotatePlayer(moveInput);
        MoveForward();
    }

    protected float CalculateSpeed(float verticalInput)
    {
        if (verticalInput == 0)
        {
            _actualSpeed += (-1 * _drag) * Time.deltaTime;
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

        return _actualSpeed;
    }

    protected float CalculateRotateSpeed(float horizontalInput)
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
            float accelerate = horizontalInput * _rotateAccelerate;
            _actualRotateSpeed += accelerate * Time.deltaTime;
            _actualRotateSpeed = Mathf.Clamp(_actualRotateSpeed, -_maxRotateSpeed, _maxRotateSpeed);
        }

        return _actualRotateSpeed;
    }

    protected void MoveForward()
    {
        _characterController.Move(transform.forward * _actualSpeed * Time.deltaTime);
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
