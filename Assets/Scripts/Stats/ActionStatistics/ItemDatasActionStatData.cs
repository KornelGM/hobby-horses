[System.Serializable]
public class ItemDatasActionStatData : AActionStatData
{
    [System.NonSerialized] public ItemData[] ItemDatas;

    public ItemDatasActionStatData(ItemData[] itemDatas)
    {
        ItemDatas = itemDatas;
    }
}
