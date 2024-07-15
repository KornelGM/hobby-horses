using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnumValueList<T1, T2> : IEnumerable where T1 : Enum where T2 : IComparable<T2>
{
    public EnumValuePair<T1, T2> this[int index] => _list[index];
    public T2 Default => DefaultValue;
    public int Count => _list.Count;

    [field: SerializeField] private T2 DefaultValue { get; set; }
    [SerializeField] private List<EnumValuePair<T1, T2>> _list = new();
    
    public EnumValueList(T2 defaultValue = default)
    {
        DefaultValue = defaultValue;
    }
    
    public EnumValueList(List<T1> keys, T2 defaultValue = default)
    {
        DefaultValue = defaultValue;
        
        foreach (T1 key in keys)
        {
            _list.Add(new EnumValuePair<T1, T2>(key, defaultValue));
        }
    }
    
    public void Sort()
    {
        _list.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));
    }
    
    public List<T1> GetKeys()
    {
        List<T1> keys = new();
        
        foreach (var pair in _list)
        {
            keys.Add(pair.Key);
        }

        return keys;
    }
    
    public List<T2> GetValues()
    {
        List<T2> values = new();
        
        foreach (var pair in _list)
        {
            values.Add(pair.Value);
        }

        return values;
    }

    public void Add(T1 key, T2 value, T2 min, T2 max, bool isClamped = true)
    {
        _list.Add(new EnumValuePair<T1, T2>(key, value, min, max, isClamped));
    }
    
    public void Remove(T1 key)
    {
        _list.RemoveAll(pair => pair.Key.Equals(key));
    }
    
    public void SetDefaultValue(T2 value)
    {
        DefaultValue = value;
    }
    
    public bool TryGetValue(T1 key, out T2 value)
    {
        value = DefaultValue;
        
        if (_list is not {Count: > 0}) return false;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;
            
            value = pair.Value.Value;
            return true;
        }

        return false;
    }
    
    public T2 GetValue(T1 key)
    {
        if (_list is not {Count: > 0}) return DefaultValue;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;
            
            return pair.Value.Value;
        }

        return DefaultValue;
    }
    
    public T2 GetMin(T1 key)
    {
        if (_list is not {Count: > 0}) return DefaultValue;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;

            return pair.Value.Min;
        }

        return DefaultValue;
    }
    
    public T2 GetMax(T1 key)
    {
        if (_list is not {Count: > 0}) return DefaultValue;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;

            return pair.Value.Max;
        }

        return DefaultValue;
    }
    
    public (T2 min, T2 max) GetMinMax(T1 key)
    {
        if (_list is not {Count: > 0}) return (DefaultValue, DefaultValue);
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;

            return (pair.Value.Min, pair.Value.Max);
        }

        return (DefaultValue, DefaultValue);
    }
    
    public bool TryGetMinMax(T1 key, out (T2 min, T2 max) minMax)
    {
        minMax = (DefaultValue, DefaultValue);
        
        if (_list is not {Count: > 0}) return false;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;

            minMax = (pair.Value.Min, pair.Value.Max);
            return true;
        }

        return false;
    }
    
    public void SetValue(T1 key, T2 value)
    {
        if (_list is not {Count: > 0}) return;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;
            
            pair.Value.Value = value;
            return;
        }
    }
    
    public T1 GetKeyByValue(T2 value)
    {
        if (_list is not {Count: > 0}) return default;
        
        foreach (var pair in _list)
        {
            if (!pair.Value.Value.Equals(value)) continue;
            
            return pair.Key;
        }

        return default;
    }
    
    public T1 GetKeyByRange(T2 value)
    {
        if (_list is not {Count: > 0}) return default;
        
        foreach (var pair in _list)
        {
            if (pair.Value.Min.CompareTo(value) <= 0 && pair.Value.Max.CompareTo(value) >= 0)
            {
                return pair.Key;
            }
        }

        return default;
    }

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
