[System.Serializable]
public class ItemDataAndGuidActionStatData : AActionStatData
{
    public string ItemDataGuid;
    public string Guid;

    public ItemDataAndGuidActionStatData(string itemDataGuid, string guid)
    {
        ItemDataGuid = itemDataGuid;
        Guid = guid;
    }
}
