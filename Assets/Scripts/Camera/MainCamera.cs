using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    [ServiceLocatorComponent] private CameraManager _cameraManager;

    public ServiceLocator MyServiceLocator { get; set; }

    public void CustomAwake()
    {
        _cameraManager.SetupCamera(GetComponent<Camera>());
    }
}
