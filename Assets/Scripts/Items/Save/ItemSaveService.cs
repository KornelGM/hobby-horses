using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class ItemSaveService : SaveService<ItemSaveData, ItemSaveData>
{
    [field: SerializeField]
    public bool StaticOnScene { get; private set; }
    [field: SerializeField] public string GUID { get; private set; } = string.Empty;
    protected override ItemSaveData SaveData(ItemSaveData data) => data;

#if UNITY_EDITOR

    [InfoBox("Use ONLY on SCENE!!",
        InfoMessageType.Warning, visibleIfMemberName: nameof(_hideInfobox))]

    [HideIf(nameof(StaticOnScene))]
    [Button("Change object to static")]
    public void ChangeToStatic()
    {
        ChangeStatic(true);
    }

    [Button("Change all children to static")]
    private void ChangeChildrenToStatic()
    {
        ItemSaveService[] childrens = GetComponentsInChildren<ItemSaveService>(true);
        foreach (ItemSaveService child in childrens)
        {
            child.ChangeToStatic();
        }
    }

    [ShowIf(nameof(StaticOnScene))]
    [Button("Disable static")]
    private void DisableStatic()
    {
        ChangeStatic(false);
    }
    private void ChangeStatic(bool shouldBeStatic)
    {
        if (StaticOnScene == shouldBeStatic) return;
        StaticOnScene = shouldBeStatic;
        if (IsPrefab())
        {
            Debug.LogError("This tool is intended for objects on scene");
            if (StaticOnScene) StaticOnScene = false;
            GUID = string.Empty;

            EditorUtility.SetDirty(this);
            return;
        }

        if (StaticOnScene)
        {
            if (string.IsNullOrEmpty(GUID)) GUID = System.Guid.NewGuid().ToString();
        }
        else
        {
            GUID = string.Empty;
        }

        Debug.Log($"Changed {gameObject.name} to be static on scene");
        EditorUtility.SetDirty(this);
    }

    private bool _hideInfobox => !StaticOnScene;

    private bool IsPrefab()
    {
        return PrefabUtility.IsPartOfPrefabAsset(gameObject) || PrefabStageUtility.GetPrefabStage(gameObject) != null;
    }
#endif

    public override ItemSaveData CollectData(ItemSaveData data)
    {
        if (!MyServiceLocator.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer, false, false))
        {
            Debug.Log($"Coldn't save object of name {gameObject.name}");
            return default;
        }

        if(data == null)
        {
            data = new(itemDataContainer.ItemData.Guid, GUID, transform);
        }
        else
        {
            data.ItemDataGUID = itemDataContainer.ItemData.Guid;
            data.PersonalGUID = GUID;
            data.Localization = new(transform);
        }

        base.CollectData(data);

        return data;
    }

    public override void Initialize(ItemSaveData save)
    {
        if(save != null)GUID = save.PersonalGUID;

        if(string.IsNullOrEmpty(GUID)) GUID = System.Guid.NewGuid().ToString();

        base.Initialize(save);
    }
}
