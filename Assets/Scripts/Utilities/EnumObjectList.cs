using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class EnumObjectList<T1, T2> : IEnumerable where T1 : Enum where T2 : Object
{
    public EnumObjectPair<T1, T2> this[int index] => _list[index];
    public int Count => _list.Count;

    [field: SerializeField] private T2 DefaultValue { get; set; }
    [SerializeField] private List<EnumObjectPair<T1, T2>> _list = new();
    
    public EnumObjectList(T2 defaultValue = default)
    {
        DefaultValue = defaultValue;
    }
    
    public EnumObjectList(List<T1> keys, T2 defaultValue = default)
    {
        DefaultValue = defaultValue;
        
        foreach (T1 key in keys)
        {
            _list.Add(new EnumObjectPair<T1, T2>(key, defaultValue));
        }
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

    public void Add(T1 key, T2 value)
    {
        _list.Add(new EnumObjectPair<T1, T2>(key, value));
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
            
            value = pair.Value;
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
            
            return pair.Value;
        }

        return DefaultValue;
    }
    
    public void SetValue(T1 key, T2 value)
    {
        if (_list is not {Count: > 0}) return;
        
        foreach (var pair in _list)
        {
            if (!pair.Key.Equals(key)) continue;
            
            pair.Value = value;
            return;
        }
    }
    
    public T1 GetKeyByValue(T2 value)
    {
        if (_list is not {Count: > 0}) return default;
        
        foreach (var pair in _list)
        {
            if (!pair.Value.Equals(value)) continue;
            
            return pair.Key;
        }

        return default;
    }

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
