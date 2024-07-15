using DG.Tweening;

public class TweenAnimationBackToDefaultRotation : BaseTweenAnimation
{
    public override Tween AnimationTween()
    {
        if(_local)
            return _objectToAnimation.DOLocalRotate(_objectToAnimation.localEulerAngles, _duration).SetEase(_ease);

        return _objectToAnimation.DORotate(_objectToAnimation.rotation.eulerAngles, _duration).SetEase(_ease);
    }
}
