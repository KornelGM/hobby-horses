using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GravityCharacterController : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public Action<float> OnJumpForceChange;

    public float MaxJumpForce => _maxJumpForce;
    public float MinJumpForce => _minJumpForce;
    public bool AbleToChargeForceOnAir => _ableToChargeForceOnAir;

    public float CurrGravity => _currGravity;
    public float JumpForce => _jumpForce;
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;

    [SerializeField, FoldoutGroup("References")] private Animator _animator;

    [SerializeField] private float _gravitySpeed = 10;
    [SerializeField] private float _maxJumpForce = 1;
    [SerializeField] private float _minJumpForce = 1;
    [SerializeField] private float _forceChargingSpeed = 1;
    [SerializeField] private AnimationCurve _forceCharging;
    [SerializeField] private bool _ableToChargeForceOnAir;

    private float _jumpForce;
    private CharacterController _characterController;
    private bool _blockGravity = false;
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

    public void ChargingForce(bool charge)
    {
        if (!charge)
            return;

        float chargingSpeed = Mathf.InverseLerp(_minJumpForce, _maxJumpForce, _jumpForce);
        float evaluateChargingSpeed = _forceCharging.Evaluate(chargingSpeed);

        _jumpForce += evaluateChargingSpeed * _forceChargingSpeed * Time.deltaTime;
        _jumpForce = Mathf.Clamp(_jumpForce, _minJumpForce, _maxJumpForce);
        OnJumpForceChange?.Invoke(_jumpForce);

    }

    public void CancelCharge()
    {
        _jumpForce = 0;
        OnJumpForceChange?.Invoke(_jumpForce);
    }

    public void Jump()
    {
        SetGravity(-_jumpForce);
        //-----
        //Debug.Log("Jump Force: " + _jumpForce + "Normalized: " + (Mathf.Clamp(_jumpForce, _minJumpForce, _maxJumpForce) / _maxJumpForce));
        if ((Mathf.Clamp(_jumpForce, _minJumpForce, _maxJumpForce) / _maxJumpForce) > 0.95 && !IsGrounded)
        {
            _animator.SetFloat("JumpForce", 0);
            _notificationsSystem.SendSuccessNotification("Essa!", NotificationType.Information);
            Debug.Log("High Jump");
        }
        else if ((Mathf.Clamp(_jumpForce, _minJumpForce, _maxJumpForce) / _maxJumpForce) <= 0.95 && (Mathf.Clamp(_jumpForce, _minJumpForce, _maxJumpForce) / _maxJumpForce) > 0.75 && !IsGrounded)
        {
            _animator.SetFloat("JumpForce", 1);
            _notificationsSystem.SendSuccessNotification("Super!", NotificationType.Information);
            Debug.Log("Mid Jump");
        }
        else
        {
            _animator.SetFloat("JumpForce", 1);
            _notificationsSystem.SendSuccessNotification("Nieüle!", NotificationType.Information);
            Debug.Log("Low Jump");
        }
    }
}
