using UnityEngine;

public interface IRigidbody
{
    public Rigidbody Rigidbody { get; set; }

    public void SetKinematicActive()
    {
        if (Rigidbody == null)
        {
            Debug.LogWarning($"Rigidbody is null in {this}");
            return;
        }
        
        Rigidbody.isKinematic = true;
    }

    public void SetKinematicInactive()
    {
        if (Rigidbody == null)
        {
            Debug.LogWarning($"Rigidbody is null in {this}");
            return;
        }
        
        Rigidbody.isKinematic = false;
    }
    
    public void SetGravityActive()
    {
        if (Rigidbody == null)
        {
            Debug.LogWarning($"Rigidbody is null in {this}");
            return;
        }
        
        Rigidbody.useGravity = true;
    }
    
    public void SetGravityInactive()
    {
        if (Rigidbody == null)
        {
            Debug.LogWarning($"Rigidbody is null in {this}");
            return;
        }
        
        Rigidbody.useGravity = false;
    }
}
