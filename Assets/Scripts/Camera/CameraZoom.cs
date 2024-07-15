using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }
	[ServiceLocatorComponent] private TimeManager _timeManager;
	[SerializeField] private CinemachineFreeLook _freeLookCamera;
	[SerializeField] private float _zoomSpeed;
	[SerializeField] private float _zoom = 0;
	[SerializeField] private float _zoomAcceleration;
	[SerializeField] private float _maxRadiusOfMiddleOrbit;
	[SerializeField] private float _minRadiusOfMiddleOrbit;
	[SerializeField] private float _maxHeightOfTopOrbit;
	[SerializeField] private float _minHeightOfTopOrbit;
	[SerializeField] private float _maxHeightOfBottomOrbit;
	[SerializeField] private float _minHeightOfBottomOrbit;


	public void Update()
	{
		Zoom();
	}

	private void Zoom()
	{
		_zoom = Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
		if (_zoom == 0) return;
		ZoomBottomOrbit(_zoom);
		ZoomMiddleOrbit(-_zoom);
		ZoomTopOrbit(-_zoom);
	}

	private void ZoomBottomOrbit(float zoomValue)
	{
		_freeLookCamera.m_Orbits[2].m_Height += zoomValue;
		_freeLookCamera.m_Orbits[2].m_Height = Mathf.Clamp(_freeLookCamera.m_Orbits[2].m_Height,
			_minHeightOfBottomOrbit, _maxHeightOfBottomOrbit);
	}

	private void ZoomMiddleOrbit(float zoomValue)
	{
		_freeLookCamera.m_Orbits[1].m_Radius += zoomValue;
		_freeLookCamera.m_Orbits[1].m_Radius = Mathf.Clamp(_freeLookCamera.m_Orbits[1].m_Radius,
			_minRadiusOfMiddleOrbit, _maxRadiusOfMiddleOrbit);
	}

	private void ZoomTopOrbit(float zoomValue)
	{
		_freeLookCamera.m_Orbits[0].m_Height += zoomValue;
		_freeLookCamera.m_Orbits[0].m_Height = Mathf.Clamp(_freeLookCamera.m_Orbits[0].m_Height,
			_minHeightOfTopOrbit, _maxHeightOfTopOrbit);
	}
}

