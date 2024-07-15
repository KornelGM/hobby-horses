using DG.Tweening;
using UnityEngine;

public class TweenAnimationRotateToTargetRotation : BaseTweenAnimation
{
    public override Tween AnimationTween()
    {
        if (_local)
        {
            return _objectToAnimation.DOLocalRotate(Target.localEulerAngles, _duration).SetEase(_ease);
        }
        return _objectToAnimation.DORotate(Target.eulerAngles, _duration).SetEase(_ease);
    }
}
