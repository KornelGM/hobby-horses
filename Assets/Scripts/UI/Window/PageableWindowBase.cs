using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class PageableWindowBase : MonoBehaviour
{
    [SerializeField] protected GameObject[] pages = null;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private TextMeshProUGUI pageNumberTMP;
    protected int _currentPage = 0;

    private void Start()
    {
        HandlePagesActivation();
        HandlePageNumberDisplay();
        HandleButtonsActivation();

        if (_previousButton == null || _nextButton == null)
            return;

        _previousButton.onClick.AddListener(() => ChangePage(-1));
        _nextButton.onClick.AddListener(() => ChangePage(1));
    }

    protected void ChangePage(int changeValue)
    {
        if (pages == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        _currentPage = Mathf.Clamp(_currentPage + changeValue, 0, pages.Length - 1);

        HandlePagesActivation();
        HandlePageNumberDisplay();
        HandleButtonsActivation();
    }

    protected void ChangeToPage(int changeToValue)
    {
        if (pages == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        _currentPage = Mathf.Clamp(changeToValue, 0, pages.Length - 1);

        HandlePagesActivation();
        HandlePageNumberDisplay();
        HandleButtonsActivation();
    }

    protected virtual void HandleButtonsActivation()
    {
        _previousButton.gameObject.SetActive(pages != null && _currentPage > 0);
        _nextButton.gameObject.SetActive(pages != null && _currentPage < pages.Length - 1);
    }

    protected virtual void HandlePageNumberDisplay()
    {
        if (pageNumberTMP == null) return;
        
        pageNumberTMP.text = $"{(_currentPage + 1)}/{pages.Length}";
    }

    protected void HandlePagesActivation()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(_currentPage == i);
        }
    }
}