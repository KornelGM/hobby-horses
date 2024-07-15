using System;
using UnityEngine;

[Serializable]
public class EnumValuePair<T1, T2> where T1 : Enum where T2 : IComparable<T2>
{
    public T1 Key
    {
        get => _key;
        set => _key = value;
    
    }
    
    public ReferenceValue<T2> Value
    {
        get => _value;
        set => _value = value;
    }

    [SerializeField] private T1 _key;
    [SerializeField] private ReferenceValue<T2> _value;

    public EnumValuePair(T1 key, T2 value, T2 min, T2 max, bool isClamped = true)
    {
        _key = key;
        _value = new ReferenceValue<T2>(value, isClamped, min, max);
    }
    
    public EnumValuePair(T1 key, T2 value)
    {
        _key = key;
        _value = new ReferenceValue<T2>(value);
    }

    public EnumValuePair(T1 key)
    {
        _key = key;
        _value = new ReferenceValue<T2>(default);
    }
}
