using UnityEngine.SceneManagement;

public class SceneSaveService : SaveService<SaveData, SaveData>, ISaveable<BaseHeaderData>
{
    protected override SaveData SaveData(SaveData data) => data;

    public override SaveData CollectData(SaveData data)
    {
        data.SceneName = SceneManager.GetActiveScene().name;
        return base.CollectData(data);
    }

    public BaseHeaderData CollectData(BaseHeaderData data)
    {
        return new();
    }

    public void Initialize(BaseHeaderData save)
    {
    }
}
