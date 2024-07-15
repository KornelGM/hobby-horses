using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentVisualizer : MonoBehaviour
{
    public bool CanBeUsed => _skinnedMeshRenderer != null && _material != null;
    public SkinnedMeshRenderer SkinnedMeshRenderer => _skinnedMeshRenderer;
    public List<(int index, int nameID)> Indexes { get; } = new ()
    {
        (0, BaseIndex),
        (1, Index1),
        (2, Index2),
        (3, Index3),
        (4, Index4),
        (5, Index5)
    };

    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private Material _material;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int BaseIndex = Shader.PropertyToID("_BaseIndex");
    private static readonly int Index1 = Shader.PropertyToID("_Index1");
    private static readonly int Index2 = Shader.PropertyToID("_Index2");
    private static readonly int Index3 = Shader.PropertyToID("_Index3");
    private static readonly int Index4 = Shader.PropertyToID("_Index4");
    private static readonly int Index5 = Shader.PropertyToID("_Index5");

    private void Awake()
    {
        TryGetComponent(out _skinnedMeshRenderer);
        
        if (_skinnedMeshRenderer == null)
        {
            Debug.LogWarning($"Renderer is null on {gameObject}");
        }

        if (_skinnedMeshRenderer != null)
        {
            _material = _skinnedMeshRenderer.material;
        }
        
        if (_material == null)
        {
            Debug.LogWarning($"Material is null on {gameObject}");
        }
    }

    public void SetValue(float value)
    {
        if (_skinnedMeshRenderer == null) return;
        
        float blendShapeWeight = _skinnedMeshRenderer.GetBlendShapeWeight(0);
        
        if (Mathf.Approximately(blendShapeWeight, value)) return;
        
        _skinnedMeshRenderer.SetBlendShapeWeight(0, Mathf.Clamp(value, 0f, 100f));
    }
    
    public void SetColor(Color color)
    {
        if (!CanBeUsed) return;
        
        _material.SetColor(BaseColor, color);
    }
    
    public void SetIndex(int index, int value)
    {
        if (!CanBeUsed) return;
        if (Indexes == null || Indexes.Count == 0) return;
        
        int nameID = Indexes.Find(tuple => tuple.index == index).nameID;
        
        _material.SetFloat(nameID, value);
    }
    
    public void ResetColor()
    {
        if (!CanBeUsed) return;
        
        _material.SetColor(BaseColor, Color.black);
    }
    
    public void ResetIndex()
    {
        if (!CanBeUsed) return;
        if (Indexes == null || Indexes.Count == 0) return;
        
        foreach (var tuple in Indexes)
        {
            _material.SetFloat(tuple.nameID, -1);
        }
    }
    
    public void ResetAll()
    {
        ResetColor();
        ResetIndex();
    }
}
