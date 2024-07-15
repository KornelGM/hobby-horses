
public class PlayerInputPauseState : State
{
    private InputManager _inputManager;
    
    public PlayerInputPauseState(InputManager inputManager)
    {
        _inputManager = inputManager;
    }
    public override void Enter()
    {
        _inputManager.PlayerInput.controllers.maps.SetMapsEnabled(true,_inputManager.PauseCategoryName);

    }

    public override void CustomUpdate()
    {
    }

    public override void CustomFixedUpdate()
    {
    }

    public override void Exit()
    {
        _inputManager?.PlayerInput?.controllers.maps.SetMapsEnabled(false,_inputManager.PauseCategoryName);
    }
}
