using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private SliderType _sliderType = SliderType.Value;
    [SerializeField] private DecimalPlaces _decimalPlaces = DecimalPlaces.One;

    private void OnEnable()
    {
        OnValueChange();
        //Invoke(nameof(OnValueChange), 0.5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void OnValueChange()
    {
        string decimalPlace = "";
        switch (_decimalPlaces)
        {
            case DecimalPlaces.Zero:
                decimalPlace = "F0";
                break;
            case DecimalPlaces.One:
                decimalPlace = "F1";
                break;
            case DecimalPlaces.Two:
                decimalPlace = "F2";
                break;
        }

        switch (_sliderType)
        {
            case SliderType.Value:
                _valueText.text = $"{_slider.value.ToString(decimalPlace)}";
                break;
            case SliderType.Percent:
                _valueText.text = $"{_slider.value.ToString(decimalPlace)}%";
                break;
            case SliderType.Weight:
                _valueText.text = $"{TextFormater.FormatWeight(_slider.value)}";
                break;
            case SliderType.Temperature:
                _valueText.text = $"{TextFormater.FormatTemperature(_slider.value)}";
                break;
        }


    }

    private enum SliderType
    {
        Value,
        Percent,
        Weight,
        Temperature,
    }
}


