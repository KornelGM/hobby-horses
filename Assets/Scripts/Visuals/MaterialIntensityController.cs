
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MaterialIntensityController : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Materials"), Tooltip("List of materials with \"Intensity\" property.")]
    List<Material> _materials = new List<Material>();
    
    private static readonly int Intensity = Shader.PropertyToID("_Intensity");

    public void UpdateIntensity(float intensity)
    {
        if (_materials is not { Count: > 0 }) return;

        foreach (var material in _materials.Where(material => material.HasFloat("_Intensity")))
        {
            float currentValue = material.GetFloat("_Intensity");
            float delay = Mathf.Abs(currentValue - intensity) * 50;

            if (delay < 0.1f)
            {
                material.SetFloat(Intensity, intensity);
                return;
            }
            else
            {
                DOTween.To(() => currentValue, x => currentValue = x, intensity, delay)
                    .OnUpdate(() => material.SetFloat(Intensity, currentValue));
            }


        }
    }
    private void OnDestroy()
    {
        if (_materials is not { Count: > 0 }) return;

        foreach (var material in _materials.Where(material => material.HasFloat("_Intensity")))
        {
            material.SetFloat(Intensity, 0);
        }
    }
}
