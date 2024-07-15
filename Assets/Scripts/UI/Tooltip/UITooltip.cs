using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private UITooltipManager _tooltipManager = null;
   
    [SerializeField] private GameObject _content = null;
    [SerializeField] private RectTransform _contentRectTransform = null;

    [SerializeField] private TextMeshProUGUI _title = null;
    [SerializeField] private TextMeshProUGUI _description = null;
    [SerializeField] private Localize _titleLocalize = null;
    [SerializeField] private Localize _descriptionLocalize = null;

    [SerializeField] private LayoutElement _contentLayoutElement = null;
    [SerializeField] private int _titleCharacterNumberLimitingSize = 40;
    [SerializeField] private int _descriptionCharacterNumberLimitingSize = 50;

    private Vector2 _offsetPerPixelX = new Vector2(0.02f, 0);
    private Vector2 _offsetPerPixelY = new Vector2(0, 0.01f);

    [SerializeField] private bool _isActive = false;

    void OnEnable()
    {
        _tooltipManager.OnShowTooltip += ShowTooltip;
        _tooltipManager.OnHideTooltip += HideTooltip;
    }

    void OnDisable()
    {
        _tooltipManager.OnShowTooltip -= ShowTooltip;
        _tooltipManager.OnHideTooltip -= HideTooltip;
        HideTooltip();
    }

    void Update()
    {
        if (!_isActive) return;

        SetPosition();
    }

    private void ShowTooltip(UITooltipContent content)
    {
        ToggleI2(content.UseI2);
        if (content.UseI2) LoadTextsUseI2(content);
        else LoadTextsNoI2(content);

        ToggleContentsActivity(content);
        SetSize();
        SetPosition();
        ActivateContent();
        StartCoroutine(UpdatePosition());

        _isActive = true;
    }

    private void HideTooltip()
    {
        DisableContent();
        ResetTooltipContent();
        StopAllCoroutines();

        _isActive = false;
    }

    private void ResetTooltipContent()
    {
        _title.text = "";
        _description.text = "";
        _titleLocalize.Term = "";
        _descriptionLocalize.Term = "";
    }

    private void ToggleContentsActivity(UITooltipContent content)
    {
        if (!content.UseI2)
        {
            if (content.Title == null || content.Title.Length == 0) _title.gameObject.SetActive(false);
            else _title.gameObject.SetActive(true);

            if (content.Description == null || content.Description.Length == 0) _description.gameObject.SetActive(false);
            else _description.gameObject.SetActive(true);
        }
        else
        {
            if (content.TitleI2 == null || content.TitleI2.Length == 0) _title.gameObject.SetActive(false);
            else _title.gameObject.SetActive(true);

            if (content.DescriptionI2 == null || content.DescriptionI2.Length == 0) _description.gameObject.SetActive(false);
            else _description.gameObject.SetActive(true);
        }
    }

    private void LoadTextsNoI2(UITooltipContent content)
    {
        _title.text = content.Title;
        _description.text = content.Description;
    }

    private void LoadTextsUseI2(UITooltipContent content)
    {
        _titleLocalize.Term = content.TitleI2;
        _descriptionLocalize.Term = content.DescriptionI2;
    }

    private void ToggleI2(bool activate)
    {
        _titleLocalize.enabled = activate;
        _descriptionLocalize.enabled = activate;
    }

    private void ActivateContent() => _content.SetActive(true);
    private void DisableContent() => _content.SetActive(false);

    private void SetSize()
    {
        int headerCharacters = 0;
        int descriptionCharacters = 0;
        if (_title.gameObject.activeSelf) headerCharacters = _title.text.Length;
        if (_description.gameObject.activeSelf) descriptionCharacters = _description.text.Length;

        if (headerCharacters > _titleCharacterNumberLimitingSize || descriptionCharacters > _descriptionCharacterNumberLimitingSize)_contentLayoutElement.enabled = true;
        else _contentLayoutElement.enabled = false;
    }

    private void SetPosition()
    {
        Vector2 newPosition = Input.mousePosition;

        float pivotX = newPosition.x / Screen.width;
        float pivotY = newPosition.y / Screen.height;

        if (pivotX < 0.5f)
        {
            pivotX = 0f;
            newPosition += _offsetPerPixelX * Screen.width;
        }
        else if (pivotX >= 0.5f)
        {
            pivotX = 1f;
            newPosition -= (_offsetPerPixelX / 2) * Screen.width;
        }

        if (pivotY < 0.85f)
        {
            pivotY = 0f;
            newPosition -= _offsetPerPixelY * Screen.height;
        }
        else if (pivotY >= 0.85f)
        {
            pivotY = 1f;
            newPosition -= _offsetPerPixelY * Screen.height;
        }

        _contentRectTransform.pivot = new Vector2(pivotX, pivotY);
        _content.transform.position = newPosition;
    }

    private IEnumerator UpdatePosition()
    {
        while(true)
        { 
            yield return 0;
            SetPosition();
        }
    }
}
