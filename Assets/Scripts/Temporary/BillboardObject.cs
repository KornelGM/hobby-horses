using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    [SerializeField] private Transform _bilboardTransform = null;
    
    private PlayerServiceLocator _playerServiceLocator = null;

    private void Update()
    {
        if (_playerServiceLocator == null)
            SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _playerServiceLocator);

        if (_playerServiceLocator == null) return;
        
        _bilboardTransform.transform.LookAt(_playerServiceLocator.transform);
        _bilboardTransform.transform.rotation = Quaternion.Euler(0, _bilboardTransform.transform.rotation.eulerAngles.y + 180, 0);
    }
}
