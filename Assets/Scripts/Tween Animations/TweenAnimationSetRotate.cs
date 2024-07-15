using DG.Tweening;
using UnityEngine;

public class TweenAnimationSetRotate : BaseTweenAnimation
{
    [SerializeField] private Vector3 _value;

    public override Tween AnimationTween()
    {
        return _objectToAnimation.DORotate(_value, _duration).SetEase(_ease);
    }
}
