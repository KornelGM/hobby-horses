using UnityEngine;


public class PopAction : BaseAction, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set ; }

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand)
    {
        Rigidbody rb = gameObject.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
           // rb.AddForce(Vector3.up * 100);
        }
        Debug.Log("kręci sięee");
    }
}
