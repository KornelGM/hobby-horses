public class ItemServiceLocator : ServiceLocator 
{
    private ItemsManager _itemsManager;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
    }

    protected override void StartComponents()
    {
        if (MyServiceLocator && MyServiceLocator.TryGetServiceLocatorComponent(out _itemsManager))
        {
            InitializeItem();
        }
        base.StartComponents();
    }

    protected override void OnDestroy()
    {
        if (MyServiceLocator && MyServiceLocator.TryGetServiceLocatorComponent(out ItemsManager itemsManager, allowToBeNull: true))
        {
            itemsManager.RemoveItem(this);
        }
        base.OnDestroy();
    }

    public void InitializeItem() => _itemsManager.InitializeItem(this);
}
