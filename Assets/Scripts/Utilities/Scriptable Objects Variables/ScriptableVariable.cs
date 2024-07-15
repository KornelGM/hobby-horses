using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableVariable<T> : ScriptableObject
{
    public Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(value, _value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    private T _value;

    public static implicit operator T(ScriptableVariable<T> reference) => reference.Value;

    public virtual void SetValue(ScriptableVariable<T> var) => Value = var.Value;
    public virtual void SetValue(T var) => Value = var;

}
