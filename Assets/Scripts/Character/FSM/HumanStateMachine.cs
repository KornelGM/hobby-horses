using UnityEngine;

public class HumanStateMachine : MoveableCharacterStateMachine
{
    public AudioPlayer Footsteps => _footsteps;
    private AudioPlayer _footsteps;
    public override void CustomStart()
    {
        base.CustomStart();
        MyServiceLocator.TryGetServiceLocatorComponent(out _footsteps);
        Footsteps.IsNotNull(this, nameof(Footsteps));
        SwitchToMoveState();
    }

    protected virtual void SwitchToMoveState()
    {
        SwitchState(new HumanMoveState(this));
    }

    //Animation event
    private void OnFootStep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (animationEvent.animatorClipInfo.clip.name.Contains("Walk"))
            {
               // Footsteps.PlayEvent(HumanAudio.FOOTSTEPS);
            }
            else
            {
               // Footsteps.PlayEvent(HumanAudio.FOOTSTEPS);
            }
        }
    }
}
