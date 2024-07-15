using DG.Tweening;
using UnityEngine;

public class TweenAnimationParabolaGoToTarget : BaseTweenAnimation
{
    [SerializeField] private float height = 0.7f;
    public override Tween AnimationTween()
    {
        if (Target == null)
        {
            Debug.LogError($"Tween Target in {_objectToAnimation.root.name} animation is NULL");

            if (_local)
                return _objectToAnimation.DOLocalMove(_objectToAnimation.localPosition, _duration).SetEase(_ease);

            return _objectToAnimation.DOMove(_objectToAnimation.position, _duration).SetEase(_ease);
        }

        Vector3 controlPoint = (_objectToAnimation.position + Target.position) / 2f;
        controlPoint += Vector3.up * height;

        Vector3[] path = new Vector3[] { _objectToAnimation.position, controlPoint, Target.position };

        return _objectToAnimation.DOPath(path, _duration, PathType.CatmullRom).SetOptions(false).SetEase(_ease);
    }
}
