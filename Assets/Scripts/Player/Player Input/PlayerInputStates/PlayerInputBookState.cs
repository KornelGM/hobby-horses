public class PlayerInputBookState : State
{
    private InputManager _inputManager;

    public PlayerInputBookState(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public override void Enter()
    {
        _inputManager.PlayerInput.controllers.maps.SetMapsEnabled(true, _inputManager.BookCategoryName);

    }

    public override void CustomUpdate()
    {
    }

    public override void CustomFixedUpdate()
    {
    }

    public override void Exit()
    {
        _inputManager?.PlayerInput?.controllers.maps.SetMapsEnabled(false, _inputManager.BookCategoryName);
    }

    public override void CustomLateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
