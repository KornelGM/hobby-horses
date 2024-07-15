using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAction : BaseAction, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set ; }

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand) { }
}
