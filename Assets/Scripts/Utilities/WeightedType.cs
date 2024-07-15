using System;
using UnityEngine;

[Serializable]
public class WeightedType<T>
{
    public int Weight => _weight;
    
    public T Type;
    
    [SerializeField, Range(0, 100)] private int _weight;

    public WeightedType(T type)
    {
        Type = type;
    }
}
