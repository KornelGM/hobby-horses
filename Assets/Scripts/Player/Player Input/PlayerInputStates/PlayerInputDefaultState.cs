public class PlayerInputDefaultState : State
{
    private InputManager _inputManager;

    public PlayerInputDefaultState(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public override void Enter()
    {
        _inputManager.PlayerInput.controllers.maps.SetMapsEnabled(true,_inputManager.DefaultCategoryName);
    }

    public override void CustomUpdate()
    {
    }

    public override void CustomFixedUpdate()
    {
    }

    public override void Exit()
    {
        _inputManager.PlayerInput.controllers.maps.SetMapsEnabled(false,_inputManager.DefaultCategoryName);
    }
}