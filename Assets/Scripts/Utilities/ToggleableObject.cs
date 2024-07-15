using UnityEngine;

public class ToggleableObject : MonoBehaviour
{
    public bool Enabled { get; private set; }

    private void Awake()
    {
        Enabled = gameObject.activeSelf;
    }

    public void Toggle()
    {
        Enabled = !Enabled;
        
        gameObject.SetActive(Enabled);
    }
    
    public void SetState(bool value)
    {
        Enabled = value;
        
        gameObject.SetActive(Enabled);
    }
    
    public void TurnOn()
    {
        Enabled = true;
        
        gameObject.SetActive(Enabled);
    }
    
    public void TurnOff()
    {
        Enabled = false;
        
        gameObject.SetActive(Enabled);
    }
}
