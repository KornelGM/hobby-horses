using System.Collections.Generic;

public class SceneServiceLocator : ServiceLocator
{
    public static SceneServiceLocator Instance { get; private set; }

    protected override void Awake()
    {
        Singletone();
        base.Awake();
    }

    void Update() => CustomUpdate();

    private void Singletone()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CollectData(SaveData data)
    {
        if (TryGetAllServiceLocatorComponentsOfType(out List<SaveService<SaveData, SaveData>> saveServices, true)) saveServices.ForEach(service => service.CollectData(data));
    }

    protected override void TryFindMyServiceLocator() { }

    private void OnDisable()
    {
        Reset();
    }

    public void Reset()
    {
        List<IManager> _managers = new();
        Utilities.CastListToOtherType(_serviceLocatorComponents,_managers);       
        _managers.ForEach(manager => manager.CustomReset());
    }
}
