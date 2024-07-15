using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class AdditionalInfoField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private float _minFontSize;

    private AdditionalInfoFieldContent _content;
    private Coroutine _toggleCoroutine;


    public void Initialize(AdditionalInfoFieldContent content)
    {
        gameObject.SetActive(true);

        if (content == null)
            return;

        _content = content;

        _title.text = _content.Title;
        _value.text = _content.Value;

        _title.color = _content.TitleColor;
        _value.color = _content.ValueColor;
    }

    public void Toggle(bool toggle, float duration, float waitBefore = 0)
    {
        _title.DOKill();
        _value.DOKill();

        if (_toggleCoroutine != null)
            StopCoroutine(_toggleCoroutine);

        if (duration == 0 && waitBefore == 0)
        {
            ForceToggle(toggle);
            return;
        }

        if (!gameObject.activeInHierarchy)
            return;

        _toggleCoroutine = StartCoroutine(ToggleCoroutine(toggle, duration, waitBefore));
    }

    private void ForceToggle(bool toggle)
    {
        _title.color = new Color(_title.color.r, _title.color.g, _title.color.b, toggle ? 1 : 0);
        _value.color = new Color(_value.color.r, _value.color.g, _value.color.b, toggle ? 1 : 0);

        if (toggle)
            SetMinFontSize(_minFontSize);
        else
            SetMinFontSize(0);
    }

    private IEnumerator ToggleCoroutine(bool toggle, float duration, float waitBefore)
    {
        yield return new WaitForSeconds(waitBefore);

        _title.DOFade(toggle ? 1 : 0, duration);
        _value.DOFade(toggle ? 1 : 0, duration).OnComplete(ChangeSize);

        void ChangeSize()
        {
            if(toggle)
                SetMinFontSize(_minFontSize);
            else
                SetMinFontSize(0);
        }
    }

    private void SetMinFontSize(float size)
    {
        _title.fontSizeMin = size;
        _value.fontSizeMin = size;
    }
}

[Serializable]
public class AdditionalInfoFieldContent
{
    public AdditionalInfoFieldContent(string title, string value, Color? titleColor = null, Color? valueColor = null)
    {
        Title = title;
        Value = value;
        TitleColor = titleColor ?? _defaultTitleColor;
        ValueColor = valueColor ?? _defaultValueColor;
    }

    public Color TitleColor;
    public Color ValueColor;
    public string Title;
    public string Value;

    private Color _defaultTitleColor = new Color(1, 0.85f, 0.6f);
    private Color _defaultValueColor = new Color(1, 0.98f, 0.9f);
}
