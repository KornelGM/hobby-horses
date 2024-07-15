using DG.Tweening;
using UnityEngine;

public class TweenAnimationGoToPoint : BaseTweenAnimation
{
    [SerializeField] private Vector3 _value;

    public override Tween AnimationTween()
    {
        if (_local)
        {
            return _objectToAnimation.DOLocalMove(_value, _duration);
        }
        return _objectToAnimation.DOMove(_value, _duration);
    }
}
