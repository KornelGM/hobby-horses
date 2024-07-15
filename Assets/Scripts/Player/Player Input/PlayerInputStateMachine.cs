using System;
using Rewired;

public class PlayerInputStateMachine : StateMachine
{
    public void SwitchToDefaultState(InputManager inputManager)
    {
        SwitchState(new PlayerInputDefaultState(inputManager));
    }
    public void SwitchToGameplayState(InputManager inputManager)
    {
        SwitchState(new PlayerInputGameplayState(inputManager));
    }

    public void SwitchToPauseState(InputManager inputManager)
    {
        SwitchState(new PlayerInputPauseState(inputManager));
    }
}