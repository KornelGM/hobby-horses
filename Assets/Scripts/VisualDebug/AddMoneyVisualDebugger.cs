public class AddMoneyVisualDebugger : VisualDebugger
{
    [ServiceLocatorComponent] private FundsManager _fundsManager;

    public override void CustomStart()
    {
        base.CustomStart();
        AddButton(this, (button) => AddMoney(10), "", "", "10");
        AddButton(this, (button) => AddMoney(100), "", "", "100");
        AddButton(this, (button) => AddMoney(1000), "", "", "1000");
    }

    private void AddMoney(int money)
    {
        _fundsManager.AddAmount(money);
    }
}
