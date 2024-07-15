using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;

    public static T Instance => instance;

    public bool InstanceExists => instance!=null;

    virtual protected void Awake(){
        if(InstanceExists&&instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        instance=(T)this;
    }

    void OnDestroy()
    {
        if(instance==this)
        {
            instance=null;
        }
    }

}
