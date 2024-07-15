using JetBrains.Annotations;
using System;
using System.Linq;
using UnityEngine;

public class CrosshairUI : MonoBehaviour, IWindow
{
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; } = 0;
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; } = false;
    public bool ShouldActivateCursor { get; set; } = false;
    public bool ShouldDeactivateCrosshair { get; set; } = false;

    [SerializeField] private CrosshairArrow[] _arrows;

    private void Awake()
    {
        DisableArrows();
    }

    public void ToggleCrosshair(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    public void DisableArrows()
    {
        foreach (var arrow in _arrows)
        {
            arrow.ToggleArrow(false);
        }
    }

    public void ActiveToggleArrow(CrosshairArrowType arrowType)
    {
        CrosshairArrow arrow = _arrows.FirstOrDefault(x => x.ArrowType == arrowType);
        arrow.ToggleArrow(true);
    }
}

[Serializable]
public class CrosshairArrow
{
    public CrosshairArrowType ArrowType;
    public GameObject ArrowGameObject;

    public void ToggleArrow(bool toggle)
    {
        ArrowGameObject.SetActive(toggle);
    }
}

public enum CrosshairArrowType
{
    Horizontal,
    Vertical, 
}
