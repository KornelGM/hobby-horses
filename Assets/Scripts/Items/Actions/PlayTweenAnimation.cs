using UnityEngine;
using UnityEngine.Events;

public class PlayTweenAnimation : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [SerializeField] private TweenAnimationsContainer _tweenAnimationsContainer;
    [SerializeField] private bool _blockPlayerDuringPlaying = true;
    [SerializeField] private UnityEvent _onEndAnimation; 

    public void PlayAnimation()
    {
        if (_tweenAnimationsContainer.TweenAnimations.Length > 0)
        {
            TweenAnimationSequencePlayer sequencePlayer = new TweenAnimationSequencePlayer();
            sequencePlayer.PlaySequence(_tweenAnimationsContainer.TweenAnimations, _onEndAnimation, 
                _blockPlayerDuringPlaying ? _playerManager.LocalPlayer : null);
        }
    }
}
