using UnityEngine;
using TMPro;
using DG.Tweening;

public class UICurrencyPanel : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private FundsManager _fundsManager = null;

    [SerializeField] private TextMeshProUGUI _valueText = null;
    [SerializeField] private float _setValueDuration = 0.3f;

    public ServiceLocator MyServiceLocator { get; set; }
    private Tween _refreshTextTween;
    private float _currentValue = 0;

    void OnEnable()
    {
        if (_fundsManager != null) _fundsManager.OnCurrencyAmountChange += UpdateValueText;
        UpdateValueText(_fundsManager.AvailableFunds,0);
    }

    void OnDisable()
    {
        if (_fundsManager != null) _fundsManager.OnCurrencyAmountChange-=UpdateValueText;
    }

    private void UpdateValueText(int value) => UpdateValueText(value, _setValueDuration);
    private void UpdateValueText(float value, float time = 0)
    {
        if (_refreshTextTween != null && _refreshTextTween.IsActive()) _refreshTextTween.Kill();
        _refreshTextTween = DOTween.To(() => _currentValue, x => { _currentValue = x; OnUpdate(x); }, value, time);

        void OnUpdate(float value)
        {
            _valueText.text = value.ToString("F0") + $"{_fundsManager.Currency}";
        }
    }
}
