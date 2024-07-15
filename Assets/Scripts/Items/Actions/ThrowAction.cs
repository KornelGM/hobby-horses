using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(DropAction))]
public class ThrowAction : BaseAction
{
    [SerializeField] private float _force = 100;
    private DropAction _drop = null;

    private void Awake()
    {
        _drop = GetComponent<DropAction>();    
    }

    public override bool Available(ServiceLocator characterServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    {
        if (playerServiceLocator.TryGetServiceLocatorComponent(out HologramController hologramController) && hologramController.Projecting) return;
        _drop.Perform(playerServiceLocator, interactionItem, caller);
        _drop.OnEndDropAnimation.Invoke();
        Throw(playerServiceLocator, caller);
    }

    private void Throw(ServiceLocator playerServiceLocator, ServiceLocator itemInHand)
    {
        CharacterHand playerHand;
        if (!playerServiceLocator.TryGetServiceLocatorComponent(out playerHand)) return;

        Vector3 forward = playerServiceLocator.transform.forward + Vector3.up;
        if(playerServiceLocator.TryGetServiceLocatorComponent(out PlayerStateMachine playerStateMachine))
        {
            forward =playerStateMachine.FirstPersonCamera.forward + Vector3.up * 0.5f;
        }

        if(playerServiceLocator.TryGetComponent(out Rigidbody rb))
        {
            forward += rb.velocity;
        }

        Vector3 dir = forward;
        AddForceOn(itemInHand.gameObject, dir * _force);
    }

    private void AddForceOn(GameObject obj, Vector3 force)
    {
        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(force);
            rb.AddTorque(rb.transform.right * 100);
        }
    }
}
