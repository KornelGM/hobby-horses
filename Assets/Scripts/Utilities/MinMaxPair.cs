using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MinMaxPair<T>
{
    public T Min
    {
        get => _min;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            if (Comparer<T>.Default.Compare(value, _max) > 0)
            {
                _min = _max;
                _max = value;
            }
            else
            {
                _min = value;
            }
        }
    }

    public T Max
    {
        get => _max;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            if (Comparer<T>.Default.Compare(value, _min) < 0)
            {
                _max = _min;
                _min = value;
            }
            else
            {
                _max = value;
            }
        }
    }
    
    [SerializeField] private T _min;
    [SerializeField] private T _max;

    public MinMaxPair(T min, T max)
    {
        Min = min;
        Max = max;
    }
}
