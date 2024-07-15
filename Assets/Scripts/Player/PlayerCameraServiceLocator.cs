using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerCameraServiceLocator : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    [field: SerializeField] public CinemachineVirtualCamera Camera { get; private set; }
    public void CustomStart()
    {
        Camera.IsNotNull(this, nameof(Camera));
    }
}
