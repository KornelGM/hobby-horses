public class AddReputationVisualDebugger : VisualDebugger
{
    [ServiceLocatorComponent] private ReputationManager _reputationManager;

    public override void CustomStart()
    {
        base.CustomStart();
        AddButton(this, (button) => AddReputation(10), "", "", "10");
        AddButton(this, (button) => AddReputation(100), "", "", "100");
        AddButton(this, (button) => AddReputation(1000), "", "", "1000");
    }

    private void AddReputation(int reputation)
    {
        _reputationManager.ChangeReputation(reputation);
    }
}
