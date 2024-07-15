using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCharacterController : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public float CurrGravity => _currGravity;
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private float _gravitySpeed = 10;
    [SerializeField] private float _jumpForce = 1;

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

    public void Jump() => SetGravity(-_jumpForce);
}
