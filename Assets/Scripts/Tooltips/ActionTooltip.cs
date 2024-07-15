using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip", menuName = "ScriptableObjects/Items/Tooltips")]
public class ActionTooltip : ScriptableObject, IActionTooltip
{
    public string Warning => LocalizedWarning.ToString();
    public string ActionName => LocalizedActionName.ToString();
    public int Priority => OutlineColorPreset == null? 0 :OutlineColorPreset.Priority;
    public Color OutlineColor => OutlineColorPreset != null ? OutlineColorPreset.Color : _transparent;
    public Color WarningColor => WarningColorPrese != null ? WarningColorPrese.Color : _transparent;

    [field: SerializeField] public bool ShowOnObject { get; set; } = true;
    [field: SerializeField] public LocalizedString LocalizedActionName { get; set; }
    [field: SerializeField] public LocalizedString LocalizedWarning { get; set; }

    [field: SerializeField] ColorHook WarningColorPrese { get; set; }

    [field: SerializeField] ColorHook OutlineColorPreset { get; set; }
    
    private static Color _transparent = new Color(0, 0, 0, 0);
    
    public void SetupTooltip(LocalizedString actionName, LocalizedString warning)
    {
        LocalizedActionName = actionName;
        LocalizedWarning = warning;
    }
    
    public void SetupTooltip(LocalizedString actionName)
    {
        if (actionName == null)
        {
            LocalizedActionName = actionName.mTerm;
            return;
        }
        
        LocalizedActionName = actionName;
    }
}