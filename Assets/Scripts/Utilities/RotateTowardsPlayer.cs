using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
	[SerializeField] private Transform _rotatingTransform;
	[SerializeField] private float _speed;
	[SerializeField] private TransformVariable _cameraTransform;
	private Vector3 _positionToLook;

	private void Update()
	{
		if (!gameObject.activeInHierarchy) return;
		Rotate();
	}

	private void Rotate() 
	{
		if (_cameraTransform == null) return;
		UpdatePositionToLook();
		_rotatingTransform.rotation = SmoothedPosition(_rotatingTransform.rotation, _positionToLook);
	}

	private void UpdatePositionToLook()
	{
		Vector3 headingToPlayer = _cameraTransform.Value.position - _rotatingTransform.position;
		_positionToLook = new Vector3(headingToPlayer.x, 0f, headingToPlayer.z);
	}

	private Quaternion SmoothedPosition(Quaternion currentRotation, Vector3 positionToLook) =>
		Quaternion.Lerp(currentRotation, TargetLookRotation(positionToLook), SpeedFactor());

	private Quaternion TargetLookRotation(Vector3 positionToLook) =>
		Quaternion.LookRotation(-positionToLook);

	private float SpeedFactor() =>
		_speed * Time.deltaTime;

}
