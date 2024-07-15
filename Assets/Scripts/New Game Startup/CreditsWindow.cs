using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindow : MonoBehaviour, IWindow
{
    public WindowManager Manager { get; set; }
    public GameObject MyObject { get => gameObject; }
    public int Priority { get; set; } = 100;
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; } = true;
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = false;
}
