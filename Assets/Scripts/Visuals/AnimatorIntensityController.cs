using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimatorIntensityController : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Animator"), Tooltip("List of Animators with a float parameter that will be controlled.")]
    List<Animator> _animators;
    
    [SerializeField, FoldoutGroup("Animator"), Tooltip("Name of the Layer in the Animator that will be controlled.")]
    string _layerName;
    
    [SerializeField, FoldoutGroup("Animator"), Tooltip(""), Range(0.1f, 5f)]
    float _intensityModifier = 1f;
    
    private int _layerIndex;

    private void Start()
    {
        if (_animators is not { Count: > 0 }) return;
        if (_animators.FirstOrDefault(animator => animator != null && animator.layerCount > 0) is not { } activeAnimator) return;
        
        _layerIndex = activeAnimator.GetLayerIndex(_layerName);
    }

    public void UpdateIntensity(float intensity)
    {
        if (_animators is not { Count: > 0 }) return;
        if (_layerIndex < 0) return;

        foreach (Animator animator in _animators)
        {
            if (!animator.gameObject.activeInHierarchy) continue;
            if (!animator.isActiveAndEnabled) continue;
            if (animator.layerCount <= 0) continue;
            
            float currentValue = animator.GetLayerWeight(_layerIndex);
            float delay = Mathf.Abs(currentValue - intensity) * 50;

            if (delay < 0.1f)
            {
                animator.SetLayerWeight(_layerIndex, intensity * _intensityModifier);
                return;
            }
            
            DOTween.To(
                    getter: () => currentValue,
                    setter: x => currentValue = x,
                    endValue: intensity * _intensityModifier,
                    duration: delay)
                .OnUpdate(() => UpdateLayerWeight(animator, _layerIndex, currentValue));
        }
    }

    private void UpdateLayerWeight(Animator animator, int layerIndex, float currentValue)
    {
        if (animator == null) return;
        if (animator.layerCount <= 0) return;
        if (animator.layerCount <= layerIndex) return;
        if (Mathf.Approximately(animator.GetLayerWeight(layerIndex), currentValue)) return;
        
        animator.SetLayerWeight(layerIndex, currentValue);
    }
    
    private void OnDestroy()
    {
        if (_animators is not { Count: > 0}) return;

        foreach (Animator animator in _animators)
        {
            if (animator == null) continue;
            if (!animator.isActiveAndEnabled) continue;
            if (animator.layerCount <= 0) continue;
            if (animator.layerCount <= _layerIndex) continue;
            
            animator.SetLayerWeight(_layerIndex, 0);
        }
    }
}
