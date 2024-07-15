using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public abstract class Hook<T> : SerializedMonoBehaviour where T : class
{
    [Tooltip("Transform that it will be saved on the Transform var asset")]
    public T Reference;

    [Tooltip("Transform Scritable var that will store at runtime a transform")]
    public ScriptableVariable<T> HookVariable;


    private void OnEnable()
    {
        UpdateHook();
    }

    private void OnDisable()
    {
        DisableHook();  //Disable it only when is not this transform
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Reference.IsNotNull(this, nameof(Reference));
        HookVariable.IsNotNull(this, nameof(HookVariable));
    }
#endif

    public virtual void UpdateHook() => HookVariable.Value = Reference;
    public virtual void DisableHook() => HookVariable.Value = null;
    public virtual void RemoveHook() => HookVariable.Value = null;
}
