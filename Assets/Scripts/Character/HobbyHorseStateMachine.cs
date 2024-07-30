using UnityEngine;

public class HobbyHorseStateMachine : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }
    [field: SerializeField] public MovementSettings MovementSettings { get; protected set; }

    public void CustomStart()
    {
        SwitchToMoveState();
    }

    public void SwitchToMoveState()
    {
        SwitchState(new HobbyHorseMovementState(this));
    }

    public void SwitchToPasueState()
    {
        SwitchState(new HobbyHorsePauseState(this));
    }

    public void SwitchState(State state)
    {
        CurrentState?.Exit();
        PreviousState = CurrentState;
        CurrentState = state;
        CurrentState.Enter();
    }
    private void Update()
    {
        CurrentState?.CustomUpdate();
    }

    private void FixedUpdate()
    {
        CurrentState?.CustomFixedUpdate();
    }
}
