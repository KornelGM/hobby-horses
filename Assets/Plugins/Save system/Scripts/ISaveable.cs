public interface ISaveable<T>
{
    public T CollectData(T data);
    public void Initialize(T save); 
}
