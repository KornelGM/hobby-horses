using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBlocker : MonoBehaviour
{
    [SerializeField] private bool blockAtStart = false;

    public bool IsCurrentlyBlocked { get; private set; }
    private Button _button;
    private ChangeCursorOnHoverUI _changeCursorOnHover;
    private PlaySoundOnHoverUI _playSoundOnHoverUI;

    private void Start()
    {
        _button = GetComponent<Button>();
        _changeCursorOnHover = GetComponent<ChangeCursorOnHoverUI>();
        _playSoundOnHoverUI = GetComponent<PlaySoundOnHoverUI>();

        if (blockAtStart) Block(true);
    }

    public void Block(bool ignoreFadeDuration = false)
    {
        if (_button == null) return;

        if (ignoreFadeDuration) _button.enabled = false;
        _button.interactable = false;
        if (ignoreFadeDuration) _button.enabled = true;
        
        if (_playSoundOnHoverUI != null) _playSoundOnHoverUI.enabled = false;
        IsCurrentlyBlocked = true;
    }

    public void Unblock()
    {
        _button.interactable = true;
        if (_playSoundOnHoverUI != null) _playSoundOnHoverUI.enabled = true;
        IsCurrentlyBlocked = false;
    }
}