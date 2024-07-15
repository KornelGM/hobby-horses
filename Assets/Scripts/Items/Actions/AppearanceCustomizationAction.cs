using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceCustomizationAction : BaseAction, IServiceLocatorComponent, IAwake
{
    /// <summary>
    /// pomysl jest taki aby akcja otwierala widok kastomizacji dzieki ktoremu gracz moglby sobie wybrac 
    /// kolor i ksztalt tego przedmiotu
    /// A moze! niech manager wezmie od gracza przedmiot na ktory sie patrzy i wtedy customizuje
    /// </summary>
    public ServiceLocator MyServiceLocator { get; set; }

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator itemInHand, ServiceLocator detectedItem)
    {
        return true;
    }

    public void CustomAwake()
    {
        //Debug.Log("Otworz okno z wyborem stylow");
    }

    //public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    //{
    //    _root.DOKill();
    //    if (_doorInfo.Opened) Close();
    //    else Open(caller.transform.position);
    //}
}
