using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

//T1 - sent type T2 - recieved type
public abstract class SaveService<T1, T2> : SerializedMonoBehaviour, IServiceLocatorComponent, ISaveable<T2>
{
    [OdinSerialize,ReadOnly] private List<ISaveable<T1>> _componentsToSave = new();

    public ServiceLocator MyServiceLocator { get; set; }

    public virtual void Initialize(T2 save)
    {
        Utilities.CastListToOtherType(MyServiceLocator.ServiceLocatorComponents, _componentsToSave);
        foreach (ISaveable<T1> component in _componentsToSave)
        {
            if (component == (ISaveable<T2>)this) continue;
            component.Initialize(SaveData(save));
        }
    }

    public virtual T2 CollectData(T2 data)
    {
        if (data == null) return default;
        foreach (ISaveable<T1> component in _componentsToSave)
        {
            if (component == (ISaveable<T2>)this) continue;
            component.CollectData(SaveData(data));
        }
        return data;
    }

    protected abstract T1 SaveData(T2 data);
}
