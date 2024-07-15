[System.Serializable]
public class WallSaveData:ItemSaveData
{
    public int MaterialID;
}

public class WallSaveService : ItemAdditionalSaveService<WallSaveData>
{

}