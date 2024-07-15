using System;
using UnityEngine;

[Serializable]
public class ObservedVariable<T>
{
    [SerializeField] private T _value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            try
            {
                dynamic newValue = (dynamic)value;
                dynamic oldValue = (dynamic)_value;
                T difference = newValue - oldValue;
                OnValueChange?.Invoke(value, difference);
            }
            catch
            {
                OnValueChange?.Invoke(value, default);
            }
        }
    }
    public Action<T, T> OnValueChange;

    public ObservedVariable()
    {
        Value = default;
    }

    public ObservedVariable(T value)
    {
        Value = value;
    }

    public static implicit operator T(ObservedVariable<T> v) => v.Value;
}
