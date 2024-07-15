using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

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
        
        if (CurrentState is not PlayerInputDefaultState || CurrentState is not PlayerMinigameState)
        {
            CurrentState?.CustomFixedUpdate();
        }
    }

    private void FixedUpdate()
    {
        CurrentState?.CustomFixedUpdate();
    }
}
