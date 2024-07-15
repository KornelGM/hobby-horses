public class PlayerSaveService : SaveService<ServerPlayerSaveData, ServerPlayerSaveData>
{
    [ServiceLocatorComponent] private PlayerQuickAccessItemsBar _playerQuickAccessItemsBar;
    [ServiceLocatorComponent] private ItemsManager _itemsManager;
    protected override ServerPlayerSaveData SaveData(ServerPlayerSaveData data) => data;

    public override ServerPlayerSaveData CollectData(ServerPlayerSaveData data)
    {
        data = new();
        return base.CollectData(data);
    }

    public override void Initialize(ServerPlayerSaveData save)
    {
        if (save == null)
        {
            base.Initialize(save);
            return;
        }

        _playerQuickAccessItemsBar.Initialize(save.InventorySaveData);
        base.Initialize(save);
    }

    public void InitializeLocal(ServerPlayerSaveData localData)
    {
        if (localData == null)
        {
            _playerQuickAccessItemsBar.Initialize(null);
            return;
        }

        _playerQuickAccessItemsBar.Initialize(localData.InventorySaveData);
    }
}
