using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private Transform _target;

    private float _angle = 0;
    private bool _enabled = false;
    private float _radius = 2;
    private bool _reversed = false;
    private float _maxAngle = 360;

    private void Awake()
    {
        ToggleVisibility(false);
        _radius = Vector3.Distance(transform.position, _target.position);
    }

    void Update()
    {
        if (!_enabled) return;
        Rotate();
    }

    private void Rotate()
    {
        _angle += _speed * Time.unscaledDeltaTime * (_reversed?-1:1);
        _angle = _angle % _maxAngle;
        Vector3 dir = transform.parent.forward;
        dir.y = 0;
        transform.position = transform.parent.position + dir * _radius - Quaternion.Euler(0,_angle,0) * dir * _radius;
        transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
    }

    public void Toggle()
    {
        if (_enabled) Disable();
        else Enable();
    }

    public void ToggleVisibility()
    {
        ToggleVisibility(!_target.gameObject.activeInHierarchy);
    }

    private void ToggleVisibility(bool enable)
    {
        _target.gameObject.SetActive(enable);
    }

    public void Enable()
    {
        _enabled = true;
    }

    public void Disable()
    {
        _angle = 0;
        _enabled = false;
        Rotate();
    }

    public void SetupSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetupRadius(float radius)
    {
        Disable();
        _radius = radius;
        _target.transform.position = transform.parent.position + transform.parent.forward * radius;
    }

    public void SetupMaxAngle(float angle)
    {
        _maxAngle = angle;
    }

    public void ToggleReverse()
    {
        _reversed = !_reversed;
    }
}
