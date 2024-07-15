using System;
using System.Collections.Generic;
using UnityEngine;

public class ManualControllerBoundsChecker : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new ();
    [SerializeField] private Material _inBoundsMaterial;
    [SerializeField] private Material _outOfBoundsMaterial;
    [SerializeField] private bool _debug;

    private void Awake()
    {
        SetUpDebugging();
    }

    private void SetUpDebugging()
    {
        if (_points is null || _points.Count == 0) return;
        
        foreach (Transform point in _points)
        {
            MeshRenderer meshRenderer = point.GetComponent<MeshRenderer>();
            
            if (meshRenderer == null) continue;
            
            meshRenderer.enabled = _debug;
        }
    }

    public bool IsInBounds(Collider collider)
    {
        if (collider is null) return false;
        if (_points is null || _points.Count == 0) return false;

        bool isInBounds = true;

        foreach (Transform point in _points)
        {
            MeshRenderer meshRenderer = point.GetComponent<MeshRenderer>();
            if (meshRenderer != null && _debug)
            {
                if (meshRenderer.material != _inBoundsMaterial) meshRenderer.material = _inBoundsMaterial;
            }
            
            if (collider is SphereCollider sphereCollider)
            {
                if (Vector3.Distance(point.position, collider.bounds.center) > sphereCollider.radius)
                {
                    isInBounds = false;
                    if (meshRenderer != null && _debug) meshRenderer.material = _outOfBoundsMaterial;
                }
            }
            else
            {
                if (collider.bounds.Contains(point.position)) continue;
                
                isInBounds = false;
                if (meshRenderer != null && _debug) meshRenderer.material = _outOfBoundsMaterial;
            }
        }
        
        return isInBounds;
    }
}
