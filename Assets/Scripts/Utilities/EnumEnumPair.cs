using System;
using UnityEngine;

[Serializable]
public class EnumEnumPair<T1, T2> where T1 : Enum where T2 : Enum
{
    public T1 Key
    {
        get => _key;
        set => _key = value;
    }
    
    public T2 Value
    {
        get => _value;
        set => _value = value;
    }

    [SerializeField] private T1 _key;
    [SerializeField] private T2 _value;

    public EnumEnumPair(T1 key, T2 value)
    {
        _key = key;
        _value = value;
    }
}
