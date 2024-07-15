using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionTooltipPanel : MonoBehaviour
{
    [SerializeField] private KeyInputToIcon _keyInputToIcon;
    [SerializeField] private InteractionType _type;
    [SerializeField] private TextMeshProUGUI _actionName;
    [SerializeField] private Image _keyBackground;
    [SerializeField] private TextMeshProUGUI _key;
    [SerializeField] private TextMeshProUGUI _warning;//Can be null

    private void Awake()
    {
        _keyInputToIcon.IsNotNull(this, nameof(_keyInputToIcon));
        _actionName.IsNotNull(this, nameof(_actionName));
        _key.IsNotNull(this, nameof(_key));
    }

    public void ShowAction(IActionTooltip tooltip)
    {
        if (tooltip == null) return;

        ShowActionName(tooltip.ActionName);
        ShowWarning(tooltip.Warning, tooltip.WarningColor);
        ShowIconKey(_type);
    }

    private void ShowIconKey(InteractionType type)
    {
        _keyBackground.enabled = true;
        ToolTipIcon icon = _keyInputToIcon.GetIcon(type);
        _keyBackground.sprite = icon.Sprite;
        _key.text = icon.Text;
    }

    private void ShowActionName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            HideActionName();
            return;
        }

        gameObject.SetActive(true);
        _actionName.text = name;
    }

    private void ShowWarning(string warning, Color color)
    {
        if (_warning == null) return;
        if (string.IsNullOrEmpty(warning))
        {
            HideWarning();
            return;
        }

        _warning.gameObject.SetActive(true);
        _warning.text = warning;
        _warning.color = color;
    }

    private void HideActionName()
    {
        if (gameObject == null || !gameObject.activeSelf) return;
        
        gameObject.SetActive(false);
        _actionName.text = "";
    }

    private void HideWarning()
    {
        if (_warning == null) return;
        _warning.gameObject.SetActive(false);
        _warning.text = "";
    }

    public void Hide()
    {
        HideActionName();
        HideWarning();
    }

    public bool CompareType(InteractionType type) => _type == type;
}
