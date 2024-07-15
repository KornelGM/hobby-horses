using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VisualDebug;

public class AccordionUI : MonoBehaviour
{
    public enum State 
    {
        Hidden,
        Shown
    }
    public string CategoryName => _categoryName;
    public Card ParentCard => _parentCard;

    [SerializeField] private TextMeshProUGUI _titleText = null;
    [SerializeField] private Transform _elementsParent = null;
    [SerializeField] private Transform _bannerElement = null;
    
    private Card _parentCard;
    private State _currentState = State.Hidden;
    private RectTransform _canvasToRebuild = null;
    private string _categoryName = "Category";
    
    public void Initialize(string categoryName,RectTransform parentElement, Card parent)
    {
        SetCategoryName(categoryName);
        _canvasToRebuild = parentElement;
        _parentCard=parent;
    }

    public void ChangeState()
    {
        _currentState = (_currentState == State.Shown ? State.Hidden : State.Shown);
        ToggleAll(_currentState);
    }

    public void AddChild(Transform child)
    {
        child.SetParent(_elementsParent);

        if (_currentState == State.Hidden)
        {
            child.gameObject.SetActive(false);
        }
    }
    
    private void ToggleAll(State state)
    {
        bool isShown = state == State.Shown;

        foreach (Transform child in _elementsParent)
        {
            if (child == _bannerElement) continue;
            child.gameObject.SetActive(isShown);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_canvasToRebuild);

        foreach (Transform child in _canvasToRebuild.GetComponentsInChildren<Transform>())
        {
            if(child.GetComponent<AccordionUI>() == null) continue;
            LayoutRebuilder.ForceRebuildLayoutImmediate(child.GetComponent<RectTransform>());
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_canvasToRebuild);
    }
    
    private void SetCategoryName(string categoryName)
    {
        _titleText.text = categoryName;
        _categoryName = categoryName;
    }
}
