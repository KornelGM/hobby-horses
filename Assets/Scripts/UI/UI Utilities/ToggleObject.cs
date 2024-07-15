using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject _toToggle;
    [SerializeField] private bool _isOn = false;

    public void Toggle()
    {
        _isOn = !_isOn;
        _toToggle.SetActive(_isOn);
    }
}
