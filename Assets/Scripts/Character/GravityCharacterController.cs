using System;
using UnityEngine;

public class GravityCharacterController : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public Action<float> OnJumpForceChange;

    public float MaxJumpForce => _maxJumpForce;

    public float CurrGravity => _currGravity;
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private float _gravitySpeed = 10;
    [SerializeField] private float _maxJumpForce = 1;
    [SerializeField] private float _forceChargingSpeed = 1;
    [SerializeField] private AnimationCurve _forceCharging;

    private float _jumpForce;
    private CharacterController _characterController;
    private bool _blockGravity = false;
    private bool _charge;
    private float _currGravity = 0;

    public bool IsGrounded => _currGravity == 0;

    public void CustomAwake()
    {
        _characterController = MyServiceLocator.GetComponent<CharacterController>();
        _characterController.IsNotNull(this, nameof(_characterController));
    }

    public void ApplyGravity(bool isGrounded)
    {
        if (_blockGravity)
            return;

        if (!isGrounded) _currGravity += _gravitySpeed * Time.deltaTime;
        else if(_currGravity > 0) _currGravity = 0;

        _characterController.Move(Vector3.down * _currGravity * Time.deltaTime);
    }

    public void SetBlockGravity(bool block)
    {
        _blockGravity = block;
    }

    public void SetGravity(float value)
    {
        _currGravity = value;
    }

    public void SwitchCharge(bool value)
    {
        _charge = value;
    }

    public void ChargingForce()
    {
        if (!_charge)
            return;

        float chargingSpeed = Mathf.InverseLerp(0, _maxJumpForce, _jumpForce);
        float evaluateChargingSpeed = _forceCharging.Evaluate(chargingSpeed);

        _jumpForce += evaluateChargingSpeed * _forceChargingSpeed * Time.deltaTime;
        _jumpForce = Mathf.Clamp(_jumpForce, 0, _maxJumpForce);
        OnJumpForceChange?.Invoke(_jumpForce);
    }

    public void CancelCharge()
    {
        SwitchCharge(false);
        _jumpForce = 0;
        OnJumpForceChange?.Invoke(_jumpForce);
    }

    public void Jump()
    {
        SetGravity(-_jumpForce);
    }
}
