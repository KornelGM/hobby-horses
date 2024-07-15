using UnityEngine;
using UnityEngine.UI;

public class ReputationStar : MonoBehaviour
{
    [SerializeField] private Image _starsFilling;
    [SerializeField] private UITooltipTrigger _uITooltip;
    private float _currentValue;
    private float _maxValue;

    public void RefreshFilling(float filling, float current, float max)
    {
        _currentValue = current;
        _maxValue = max;

        UITooltipContent content = new();
        content.Title = $"{_currentValue}/{_maxValue}";

        _uITooltip.SetTooltipContent(content);

        float scaledValue = Mathf.InverseLerp(0, 1, filling);
        _starsFilling.fillAmount = scaledValue;
    }
}
