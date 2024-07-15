using UnityEngine;

public class ObjectToggler: MonoBehaviour
{
	[SerializeField] private GameObject _gameObject1;
	[SerializeField] private GameObject _gameObject2;
	[SerializeField] private GameObject _gameObject3;
	[SerializeField] private GameObject _gameObject4;
	[SerializeField] private GameObject _gameObject5;
	[SerializeField] private GameObject _gameObject6;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			_gameObject1.SetActive(!_gameObject1.activeSelf);

		if(Input.GetKeyDown(KeyCode.Alpha2))
			_gameObject2.SetActive(!_gameObject2.activeSelf);

		if(Input.GetKeyDown(KeyCode.Alpha3))
			_gameObject3.SetActive(!_gameObject3.activeSelf);

		if(Input.GetKeyDown(KeyCode.Alpha4))
			_gameObject4.SetActive(!_gameObject4.activeSelf);

		if(Input.GetKeyDown(KeyCode.Alpha5))
			_gameObject5.SetActive(!_gameObject5.activeSelf);

		if(Input.GetKeyDown(KeyCode.Alpha6))
			_gameObject6.SetActive(!_gameObject6.activeSelf);
	}
}
