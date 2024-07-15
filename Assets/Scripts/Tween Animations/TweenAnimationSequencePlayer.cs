using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TweenAnimationSequencePlayer
{
    private Transform _transformToAnimate;

    public void SetTransformToAnimate(Transform transform)
    {
        _transformToAnimate = transform;
    }


    public void PlaySequence(BaseTweenAnimation[] tweenAnimations, UnityEvent onEndAnimation = null, ServiceLocator caller = null, Transform target = null)
    {

        PlayerServiceLocator playerServiceLocator = caller as PlayerServiceLocator;
        if (playerServiceLocator != null)
        {
            if (playerServiceLocator.TryGetServiceLocatorComponent(out PlayerInputBlocker blocker, false, false))
            {
                InputBlockerSettings blockerSettings = new InputBlockerSettings(this);
                blocker.Block(blockerSettings);

                if (onEndAnimation == null)
                {
                    onEndAnimation = new UnityEvent ();
                }

                onEndAnimation?.AddListener(() => { blocker.TryUnblock(this); });
            }
        }

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < tweenAnimations.Length; i++)
        {
            if(target != null)
                tweenAnimations[i].SetTarget(target);

            if (tweenAnimations[i].ObjectToAnimation == null && _transformToAnimate != null)
                tweenAnimations[i].SetObjectToAnimation(_transformToAnimate);

            if (tweenAnimations[i].ObjectToAnimation == null)
                continue;

            if (tweenAnimations[i].WaitBefore)
                sequence.AppendInterval(tweenAnimations[i].WaitingTime);

            if (tweenAnimations[i].ObjectToAnimation != null && tweenAnimations[i].DoKill)
                tweenAnimations[i].ObjectToAnimation.DOKill();

            if (i != tweenAnimations.Length - 1)
            {
                if (tweenAnimations[i].Append)
                    sequence.Append(tweenAnimations[i].AnimationTween());
                else if (tweenAnimations[i].Join)
                    sequence.Join(tweenAnimations[i].AnimationTween());
            }
            else
            {
                if (tweenAnimations[i].Append)
                    sequence.Append(tweenAnimations[i].AnimationTween()).OnComplete(() => onEndAnimation?.Invoke());
                else if (tweenAnimations[i].Join)
                    sequence.Join(tweenAnimations[i].AnimationTween()).OnComplete(() => onEndAnimation?.Invoke());
            }
        }
    }
}
