using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReferenceValue<T> where T : IComparable<T>
{
    public T Min
    {
        get => _min;
        set => _min = value;
    }
    
    public T Max
    {
        get => _max;
        set => _max = value;
    }

    public bool IsClamped => _isClamped;
    
    [SerializeField] private T _value;
    [SerializeField] private bool _isClamped;
    [SerializeField] private T _min;
    [SerializeField] private T _max;
    
    public event Action<T> OnValueChanged;
    public event Action OnMinValueReached;
    public event Action OnMaxValueReached;
    public event Action<T, T> OnValueDecreased;
    public event Action<T, T> OnValueIncreased;

    public T Value
    {
        get => _value;
        set
        {
            T oldValue = _value;
            
            if (_value.CompareTo(value) > 0)
            {
                OnValueDecreased?.Invoke(oldValue, value);
            }
            else if (_value.CompareTo(value) < 0)
            {
                OnValueIncreased?.Invoke(oldValue, value);
            }
            
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
            
            if (_isClamped)
            {
                if (value.CompareTo(_min) <= 0)
                {
                    _value = _min;
                    
                    if (!EqualityComparer<T>.Default.Equals(oldValue, _value))
                    {
                        OnMinValueReached?.Invoke();
                    }
                    
                    return;
                }
                
                if (value.CompareTo(_max) >= 0)
                {
                    _value = _max;
                    
                    if (!EqualityComparer<T>.Default.Equals(oldValue, _value))
                    {
                        OnMaxValueReached?.Invoke();
                    }
                    
                    return;
                }
            }
        }
    }

    public ReferenceValue(ReferenceValueSaveData<T> data)
    {
        _value = data.Value;
        _isClamped = data.IsClamped;
        _min = data.Min;
        _max = data.Max;
    }

    public ReferenceValue(T value, bool isClamped = false, T min = default, T max = default)
    {
        _value = value;
        _isClamped = isClamped;
        _min = min;
        _max = max;
    }
    
    public override string ToString() => _value.ToString();

    public void Clamp(T min, T max, bool isClamped = true)
    {
        _isClamped = isClamped;
        _min = min;
        _max = max;
    }

    public void Unclamp()
    {
        _isClamped = false;
        _min = default;
        _max = default;
    }
    
    public void ClearAllActions()
    {
        ClearValueChangedAction();
        ClearMinMaxValueReachedActions();
    }
    
    public void ClearValueChangedAction()
    {
        OnValueChanged = null;
    }
    
    public void ClearMinMaxValueReachedActions()
    {
        OnMinValueReached = null;
        OnMaxValueReached = null;
    }
    
    public ReferenceValue<T> Clone() => new(_value, _isClamped, _min, _max);
    
    public static implicit operator T(ReferenceValue<T> referenceValue) => referenceValue.Value;
    
    public static ReferenceValue<T> operator +(ReferenceValue<T> value1, T value2)
    {
        value1.Value = (dynamic)value1.Value + value2;
        return value1;
    }
    
    public static ReferenceValue<T> operator -(ReferenceValue<T> value1, T value2)
    {
        value1.Value = (dynamic)value1.Value - value2;
        return value1;
    }
    
    public static ReferenceValue<T> operator *(ReferenceValue<T> value1, T value2)
    {
        value1.Value = (dynamic)value1.Value * value2;
        return value1;
    }
    
    public static ReferenceValue<T> operator /(ReferenceValue<T> value1, T value2)
    {
        value1.Value = (dynamic)value1.Value / value2;
        return value1;
    }
}

[Serializable]
public class ReferenceValueSaveData<T> where T : IComparable<T>
{
    public T Value;
    public T Min;
    public T Max;
    public bool IsClamped;

    public ReferenceValueSaveData(ReferenceValue<T> referenceValue)
    {
        Value = referenceValue.Value;
        Min = referenceValue.Min;
        Max = referenceValue.Max;
        IsClamped = referenceValue.IsClamped;
    }
}

[Serializable]
public class IntReferenceValueSaveData : ReferenceValueSaveData<int>
{
    public IntReferenceValueSaveData(ReferenceValue<int> referenceValue) : base(referenceValue)
    {
    }
}

[Serializable]
public class FloatReferenceValueSaveData : ReferenceValueSaveData<float>
{
    public FloatReferenceValueSaveData(ReferenceValue<float> referenceValue) : base(referenceValue)
    {
    }
}