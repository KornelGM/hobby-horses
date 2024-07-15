using DG.Tweening;
using UnityEngine;

public class TweenAnimationGoToTarget : BaseTweenAnimation, IServiceLocatorComponent
{
    [SerializeField] private Vector3 _offset = Vector3.zero;

    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private float _playerRightOffset;
    [SerializeField] private float _playerForwardOffset;

    [ServiceLocatorComponent] private PlayerManager _playerManager;
    public override Tween AnimationTween()
    {
        if (Target == null)
        {
            Debug.LogError($"Tween Target in {_objectToAnimation.root.name} animation is NULL");

            if (_local)
                return _objectToAnimation.DOLocalMove(_objectToAnimation.localPosition, _duration).SetEase(_ease);

            return _objectToAnimation.DOMove(_objectToAnimation.position, _duration).SetEase(_ease);
        }

        if (_local)
            return _objectToAnimation.DOLocalMove(Target.localPosition + _offset 
                + (_playerRightOffset * _playerManager.LocalPlayer.transform.right)
                + (_playerForwardOffset * _playerManager.LocalPlayer.transform.forward), _duration).SetEase(_ease);
        
        return _objectToAnimation.DOMove(Target.position + _offset
            + (_playerRightOffset * _playerManager.LocalPlayer.transform.right)
            + (_playerForwardOffset * _playerManager.LocalPlayer.transform.forward), _duration).SetEase(_ease);
    }
}
