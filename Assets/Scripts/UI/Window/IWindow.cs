using System;
using UnityEngine;

public interface IWindow
{
    public WindowManager Manager { get; set; }
    public GameObject MyObject { get; }

    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; }
    public void DeleteWindow() { Manager.DeleteWindow(this); }
    public void OnDeleteWindow() { }
}