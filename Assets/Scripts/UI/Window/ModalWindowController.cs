using System;
using I2.Loc;
using JetBrains.Annotations;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalWindowController : MonoBehaviour, IWindow
{
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; } = 100;
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; } = true;
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = true;
    public Action OnDeleteWindowAction;

    [SerializeField] private GameObject[] buttons;
    [SerializeField] private TextMeshProUGUI titleTMPro;
    [SerializeField] private TextMeshProUGUI descriptionTMPro;

    public string Title
    {
        set => titleTMPro.text = value;
    }

    public string Description
    {
        set => descriptionTMPro.text = value;
    }

    private int _activatedButtons = 0;

    private void Awake()
    {
        buttons.ForEach(button => button.SetActive(false));
    }

    public ModalWindowController AddButton(string content, Action action, bool shouldCloseModal = true, [CanBeNull] ColorHook colorHook = null )
    {
        if (_activatedButtons >= buttons.Length)
        {
            Debug.LogError($"You cannot add more than {buttons.Length} to modal window.");
            return this;
        }

        if (shouldCloseModal) action += () => Manager.DeleteWindow(this);
        
        
        buttons[_activatedButtons].SetActive(true);
        buttons[_activatedButtons].GetComponentInChildren<TextMeshProUGUI>().text = content;
        Button buttonComponent =  buttons[_activatedButtons].GetComponent<Button>();
        buttonComponent.onClick.AddListener(new UnityAction(action));
        if (colorHook != null)
        {
            var buttonColorsStruct = buttonComponent.colors;
            buttonColorsStruct.normalColor = colorHook.Color;
            buttonComponent.colors = buttonColorsStruct;
        }

        _activatedButtons++;
       
        return this;
    }

    public void OnDeleteWindow() 
    {
        OnDeleteWindowAction?.Invoke();
    }

}