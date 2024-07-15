using DG.Tweening;

public class TweenAnimationBackToDefaultPosition : BaseTweenAnimation
{
    public override Tween AnimationTween()
    {
        if(_local)
            return _objectToAnimation.DOLocalMove(_objectToAnimation.localPosition, _duration).SetEase(_ease);

        return _objectToAnimation.DOMove(_objectToAnimation.position, _duration).SetEase(_ease);
    }
}
