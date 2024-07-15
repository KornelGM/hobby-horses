using UnityEngine;

public class ComponentToggle : MonoBehaviour
{
    public Behaviour component; // The component you want to toggle (e.g., Renderer, Collider, etc.)
    public float toggleInterval = 3f;   // The time interval for toggling the component
    private bool isComponentActive = true;

    private void Start()
    {
        if (component == null)
        {
            Debug.LogError("Component to toggle is not assigned!");
            enabled = false; // Disable the script
        }
        else
        {
            InvokeRepeating("ToggleComponent", toggleInterval, toggleInterval); // Call ToggleComponent based on the specified interval
        }
    }

    private void ToggleComponent()
    {
        component.enabled = isComponentActive;
        isComponentActive = !isComponentActive;
    }
}
