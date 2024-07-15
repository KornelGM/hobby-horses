using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour, IWindow, IServiceLocatorComponent
{
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; } = false;
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = true;
    public ServiceLocator MyServiceLocator { get; set; }
}
