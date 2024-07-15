using UnityEngine;

public class CameraOrbits : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }
	
	[SerializeField] private TransformVariable _cameraOrbitsGameObjectTransform;
	[SerializeField] private Vector3 _positionOfCameraOrbitsGameObjectWhenPlayerOnFoot;
	[SerializeField] private Vector3 _positionOfCameraOrbitsGameObjectWhenPlayerIsOnHorse;

	public void SetCameraOrbitsGameObjectPositionOnMount() =>
		_cameraOrbitsGameObjectTransform.Value.localPosition = _positionOfCameraOrbitsGameObjectWhenPlayerIsOnHorse;
	
	public void SetCameraOrbitsGameObjectPositionOnFoot() =>
		_cameraOrbitsGameObjectTransform.Value.localPosition = _positionOfCameraOrbitsGameObjectWhenPlayerOnFoot;
	
	public void SetCameraOrbitsGameObjectPosition(Vector3 position) => 
		_cameraOrbitsGameObjectTransform.Value.position = position;
}
