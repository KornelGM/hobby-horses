[System.Serializable]
public class ItemDataActionStatData : AActionStatData
{
     public string ItemDataGuid;

    public ItemDataActionStatData(string itemData)
    {
        ItemDataGuid = itemData;
    }
}
