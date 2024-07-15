public class ItemAdditionalSaveService<T> : ItemSaveService, IAwake where T: ItemSaveData, new()
{
    private AdditionalSaveService<T, ItemSaveData> _additional = default;

    public void CustomAwake()
    {
        _additional = new(MyServiceLocator, this);
    }

    public override ItemSaveData CollectData(ItemSaveData data)
    {
        T wallData = CreateNewData(data);
        wallData = base.CollectData(wallData) as T;
        return _additional.CollectData(wallData);
    }

    public override void Initialize(ItemSaveData save)
    {
        base.Initialize(save);
        _additional.Initialize(save);
    }

    protected virtual T CreateNewData(ItemSaveData data) => new T();
}
