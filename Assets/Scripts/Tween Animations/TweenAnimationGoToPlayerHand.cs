using DG.Tweening;
using UnityEngine;

public class TweenAnimationGoToPlayerHand : BaseTweenAnimation, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private PlayerManager _playerManager;

    public override Tween AnimationTween()
    {
        if (!_playerManager.LocalPlayer.TryGetServiceLocatorComponent(out CharacterHand characterHand)) 
        {
            return null;
        }

        Transform parent;
        if (_local)
        {
            Vector3 localDestinationPos;
            Quaternion localRotation;
            (localDestinationPos, localRotation, parent) = characterHand.GetLocalHandDestination(MyServiceLocator);

            return _objectToAnimation.DOLocalMove(localDestinationPos, _duration);
        }

        Vector3 destinationPos;
        Quaternion rotation;
        (destinationPos, rotation, parent) = characterHand.GetHeldDestination(MyServiceLocator);
        return _objectToAnimation.DOMove(destinationPos, _duration);
    }
}
