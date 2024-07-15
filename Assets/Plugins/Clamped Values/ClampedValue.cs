using System;
using UnityEngine;

[Serializable]  
public class ClampedValue
{
    public ClampedValue(float minValue = 0, float maxValue = 100, float value = 100)
    {
        _minValue = minValue;
        _maxValue = maxValue;
        _value = value;
    }

    public ClampedValue(int minValue = 0, int maxValue = 100, int value = 100) : this((float)minValue, (float)maxValue, (float)value) { }
    public ClampedValue(ClampedValue clampedValue) : this(clampedValue.MinValue, clampedValue.MaxValue, clampedValue.Value) { }
    
    public void SetValueSilent(float value) => _value = value;

    public float Value
    {
        get => _value;
        set 
        {
            float oldValue = _value;
            _value = Mathf.Clamp(value, MinValue, MaxValue);
            OnValueChanged?.Invoke(oldValue, _value);

            if (oldValue != _value && _value == _maxValue) OnReachedMaxValue?.Invoke();
            if (oldValue != _value && _value == _minValue) OnReachedMinValue?.Invoke();
        }
    }

    public event Action<float, float> OnValueChanged;
    public event Action OnReachedMaxValue;
    public event Action OnReachedMinValue;

    public float MinValue
    {
        get => _minValue;
        set => _minValue = value;
    }

    public float MaxValue
    {
        get => _maxValue;
        set => _maxValue = value;
    }

    [SerializeField] private float _minValue;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _value;

    public void OnInspectorChanged()
    {
        OnValueChanged?.Invoke(_value, _value);
    }

    public float NormalizedValue() => Mathf.InverseLerp(_minValue, _maxValue, _value);
    public float GetValue(float normalized) => Mathf.Lerp(_minValue, _maxValue, normalized);
    public float SetValue(float normalized) => Value = GetValue(normalized);

    public void SetMinMax(float min, float max)
    {
        _minValue = min;
        _maxValue = max;
    }

    public static ClampedValue operator + (ClampedValue value1, float value2)
    {
        value1.Value += value2;
        value1.Clamp();
        return value1;
    }

    public static ClampedValue operator -(ClampedValue value)
    {
        value.Value = -value.Value;
        value.Clamp(); 
        return value;
    }

    public static ClampedValue operator -(ClampedValue value1, float value2)
    {
        value1.Value -= value2;
        value1.Clamp();
        return value1;
    }

    public static ClampedValue operator *(ClampedValue value1, float value2)
    {
        value1.Value *= value2;
        value1.Clamp();
        return value1;
    }

    public static ClampedValue operator /(ClampedValue value1, float value2)
    {
        value1.Value /= value2;
        value1.Clamp();
        return value1;
    }

    public static implicit operator float(ClampedValue d) => d.Value;
}
