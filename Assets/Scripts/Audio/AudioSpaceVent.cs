using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpaceVent : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    private bool _isOpen;
    public bool IsOpen => _isOpen;

    public void ToggleOpen(bool open) => _isOpen = open;
}
