public class HobbyHorsePauseState : State
{
    private HobbyHorseStateMachine _stateMachine;
    private InputManager _inputManager;

    public HobbyHorsePauseState(HobbyHorseStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _inputManager);
    }

    public override void Enter()
    {
        _inputManager.SwitchInputMap(true, _inputManager.PauseCategoryName);
    }

    public override void Exit()
    {
        _inputManager.SwitchInputMap(false, _inputManager.PauseCategoryName);
    }

    public override void CustomUpdate()
    {

    }

    public override void CustomFixedUpdate()
    {

    }

    public override void CustomLateUpdate()
    {

    }
}
