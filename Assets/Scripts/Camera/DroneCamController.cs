using UnityEngine;

public class DroneCamController : MonoBehaviour, IServiceLocatorComponent
{
    private bool _active = false;
    private float _speedModifier = 0f;
    private readonly float _moveSpeed = 15f;
    private readonly float _lerpTime = 10f;

    public ServiceLocator MyServiceLocator { get; set; }

    private void Update()
    {
        if (_active)
        {
            SetPosition();
            SetRotation();
        }
    }

    public void Enable()
    {
        _active = true;
    }

    public void Disable()
    {
        _active = false;
    }

    private void SetPosition()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        bool moveUp = Input.GetKey(KeyCode.E);
        bool moveDown = Input.GetKey(KeyCode.Q);

        move += new Vector3(0, moveUp?1:0 - (moveDown?1:0), 0);

        _speedModifier = Mathf.Lerp(_speedModifier, move.sqrMagnitude == 0 ? 0f : 1f, Time.unscaledDeltaTime * _lerpTime);

        transform.position +=  transform.rotation * move * _moveSpeed * _speedModifier * Time.unscaledDeltaTime ;
    }

    private void SetRotation()
    {
        if (!Input.GetKey(KeyCode.Mouse1))
            return;

        Vector3 lookRotation = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse ScrollWheel"));

        Vector3 euler = transform.rotation.eulerAngles;

        transform.rotation *= Quaternion.Euler(lookRotation);
        Vector3 newEuler = transform.rotation.eulerAngles;
        newEuler.x = Mathf.Clamp(newEuler.x > 180f ? newEuler.x - 360f : newEuler.x, -85f, 85f);

        transform.rotation = Quaternion.Euler(newEuler.x, newEuler.y, euler.z);
        transform.rotation *= Quaternion.Euler(0f, 0f, lookRotation.z);
    }
}
