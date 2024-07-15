using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

//T1 - sent type T2 - recived type
public class AdditionalSaveService<T1, T2> : IServiceLocatorComponent, ISaveable<T2> where T1 : class
{
    [OdinSerialize, ReadOnly] private List<ISaveable<T1>> _componentsToSave = new();

    public ServiceLocator MyServiceLocator { get; set; }
    private object _skipComponent;

    public AdditionalSaveService(ServiceLocator myServiceLocator, object caller)
    {
        MyServiceLocator = myServiceLocator;
        _skipComponent = caller;
    }

    public virtual void Initialize(T2 save)
    {
        Utilities.CastListToOtherType(MyServiceLocator.ServiceLocatorComponents, _componentsToSave);

        foreach (ISaveable<T1> component in _componentsToSave)
        {
            if (component == _skipComponent) continue;
            component.Initialize(SaveData(save));
        }
    }

    public virtual T2 CollectData(T2 data)
    {
        if (data == null) return default;
        foreach (ISaveable<T1> component in _componentsToSave)
        {
            if (component == _skipComponent) continue;
            component.CollectData(SaveData(data));
        }
        return data;
    }

    protected T1 SaveData(T2 data) 
    {
        if (data == null) return null;

        if (Utilities.TryCast(data, out T1 result))
        { 
            return result;
        }

        Debug.Log($"Couldn't load all data in {_skipComponent}\nCouldn't cast to type {typeof(T1)}");
        return null;
    }
}
