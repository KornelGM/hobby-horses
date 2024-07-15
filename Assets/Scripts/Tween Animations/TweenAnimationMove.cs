using DG.Tweening;
using UnityEngine;

public class TweenAnimationMove : BaseTweenAnimation
{
    [SerializeField] private Vector3 _value;

    public override Tween AnimationTween()
    {
        if (_local)
        {
            return _objectToAnimation.DOLocalMove(_objectToAnimation.localPosition + _value, _duration);
        }
        return _objectToAnimation.DOMove(_objectToAnimation.position + _value, _duration);
    }
}
