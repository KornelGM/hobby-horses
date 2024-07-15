using UnityEngine;

public interface IActionTooltip
{
    public bool ShowOnObject { get; set; }
    public string ActionName { get; }
    public string Warning { get; }
    public int Priority { get; }
    public Color WarningColor { get; }
    public Color OutlineColor { get; }
}