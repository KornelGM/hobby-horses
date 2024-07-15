using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localizer : MonoBehaviour, IServiceLocatorComponent, ISaveable<LocalizationSaveInfo>
{
    public ServiceLocator MyServiceLocator { get ; set; }

    public LocalizationSaveInfo CollectData(LocalizationSaveInfo data)
    {
        GetPositionAndRotationOfObject(data);
        return data;
    }

    protected virtual void GetPositionAndRotationOfObject(LocalizationSaveInfo data)
    {
        data.StartPostion = new(MyServiceLocator.transform.position);
        data.StartRotation = new(MyServiceLocator.transform.rotation);
    }

    public void Initialize(LocalizationSaveInfo save)
    {
        if(save == null) return;
        SetPositionAndRotationOfObject(save);
    }

    protected virtual void SetPositionAndRotationOfObject(LocalizationSaveInfo save)
    {
        MyServiceLocator.transform.SetPositionAndRotation(save.StartPostion.Vector3, save.StartRotation.Quaternion);
    }

}
