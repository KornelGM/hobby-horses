
using UnityEngine;

public class ClampedValueUser : MonoBehaviour
{
    [SerializeField] private ClampedValueSlider _valueListener;
    public ClampedValue _exampleValue = new ClampedValue(0,100,100);
    public ClampedValue[] _exampleValues = new ClampedValue[2];

    private void Awake()
    {
        _valueListener.Initialize(_exampleValue);
    }

    [ContextMenu("Refresh")]
    private void Refresh()
    {
        _exampleValue.OnInspectorChanged();
    }
}
