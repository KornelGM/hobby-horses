[System.Serializable]
public class IdToObjectPair<T>
{
    public IdToObjectPair(string id, T obj)
    {
        Id = id;
        Obj = obj;
    }

    public string Id;
    public T Obj;
}