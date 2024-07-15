using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TweenAnimationScale : BaseTweenAnimation
{
    [SerializeField] private Vector3 _value;
    [SerializeField] private bool _punchScale;

    [ShowIf("_punchScale")]
    [SerializeField] private int _vibrato;

    public override Tween AnimationTween()
    {
        if (_punchScale)
        {
            return _objectToAnimation.DOPunchScale(_value, _duration, vibrato: _vibrato).SetEase(_ease);
        }
        return _objectToAnimation.DOScale(_value, _duration).SetEase(_ease);
    }
}
