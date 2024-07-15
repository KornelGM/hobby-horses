using DG.Tweening;
using UnityEngine;

public class TweenAnimationRotate : BaseTweenAnimation
{
    [SerializeField] private Vector3 _value;

    public override Tween AnimationTween()
    {
        if (_local)
        {
            return _objectToAnimation.DOLocalRotate(_objectToAnimation.localEulerAngles + _value, _duration).SetEase(_ease);
        }
        return _objectToAnimation.DORotate(_objectToAnimation.eulerAngles + _value, _duration).SetEase(_ease);
    }
}
