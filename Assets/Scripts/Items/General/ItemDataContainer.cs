using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class ItemDataContainer : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }
    public InteractionInfo InteractionInfo => _interactionInfo;
    [SerializeField] private InteractionInfo _interactionInfo;
    [SerializeField] private ItemDataDatabase _itemDataDatabase;

    public ItemData ItemData;


    public void CustomAwake()
    {
        ItemData.IsNotNull(this, nameof(ItemData));
    }

#if UNITY_EDITOR

    #region ItemData Tools

    [ShowIf(nameof(CanCreateItemData))]
    [Button("Create Item Data")]
    private void CreateItemData()
    {
        if (!CanCreateItemData()) return;
        TryGetComponent(out ServiceLocator serviceLocator);
        ItemData = _itemDataDatabase.GetItemData(serviceLocator);

        if (ItemData == null)
        {
            string path = "Assets/Scriptable Objects/Items/Datas/" + name + ".asset";
            ItemData = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(ItemData, path);
            EditorUtility.FocusProjectWindow();
            EditorUtility.SetDirty(gameObject);
        }

        if (!_itemDataDatabase.EntryList.Contains(ItemData))
        {
            _itemDataDatabase.EntryList.Add(ItemData);
            _itemDataDatabase.RefreshDictionary();
            EditorUtility.SetDirty(_itemDataDatabase);
        }

        AssignThisPrefabToItemData();
        Selection.activeObject = ItemData;
        AssetDatabase.SaveAssets();
        AssetDatabase.SaveAssetIfDirty(ItemData);
    }

    [ShowIf(nameof(CanAssignThisPrefabToItemData))]
    [Button("Assign this prefab to Item Data")]
    private void AssignThisPrefabToItemData()
    {
        if (!CanAssignThisPrefabToItemData()) return;
        if (ItemData.Prefab == null) TryGetComponent(out ItemData.Prefab);
    }

    #endregion

    #region Offset tools
    [BoxGroup("Offset")]
    [ShowIf(nameof(CanAttachToCamera))]
    [Button("Attach To Camera")]
    public void AttachToCamera()
    {
        if (!OffsetToolAvailable()) return;

        TestOffset();
        ItemData.AttachedToCamera = true;
        SetupOffsetAndRotation();
    }

    [BoxGroup("Offset")]
    [ShowIf(nameof(CanAttachToPlayer))]
    [Button("Attach To Player")]
    public void AttachToPlayer()
    {
        if (!OffsetToolAvailable()) return;

        TestOffset();
        ItemData.AttachedToCamera = false;
        SetupOffsetAndRotation();
    }


    [BoxGroup("Offset")]
    [ShowIf(nameof(OffsetToolAvailable))]
    [Button("Setup offset")]
    public void SetupOffsetAndRotation()
    {
        if (!OffsetToolAvailable()) return;

        Transform parent = ItemData.AttachedToCamera ? Camera.main.transform : FindObjectOfType<SceneServiceLocator>()?.transform;
        if (parent == null)
        {
            Debug.LogWarning("Couldn't find parent");
            return;
        }

        ItemData.Offset = parent.InverseTransformPoint(transform.position);
        ItemData.Rotation = Quaternion.LookRotation(parent.InverseTransformVector(transform.forward), parent.InverseTransformVector(transform.up)).eulerAngles;
        EditorUtility.SetDirty(ItemData);
        AssetDatabase.SaveAssetIfDirty(ItemData);
        Debug.LogWarning("<color=#00FF00> Successfullty assigned offset and rotation </color>");
    }


    [BoxGroup("Offset")]
    [ShowIf(nameof(OffsetToolAvailable))]
    [Button("Test offset")]
    public void TestOffset()
    {
        if (!OffsetToolAvailable()) return;

        Transform parent = ItemData.AttachedToCamera ? Camera.main.transform : FindObjectOfType<SceneServiceLocator>()?.transform;
        transform.position = parent.TransformPoint(ItemData.Offset);
        transform.rotation = parent.rotation * Quaternion.Euler(ItemData.Rotation);
    }

    #endregion

    #region Tool Utilities
    private bool OffsetToolAvailable()
    {
        if (ItemData == null) return false;
        if (IsPrefab()) return false;
        return true;
    }

    private bool CanAttachToPlayer()
    {
        if (!OffsetToolAvailable()) return false;
        if (ItemData == null) return false;
        return ItemData.AttachedToCamera;
    }

    private bool CanAttachToCamera()
    {
        if (!OffsetToolAvailable()) return false;
        if (ItemData == null) return false;
        return !ItemData.AttachedToCamera;
    }

    private bool CanAssignThisPrefabToItemData()
    {
        if (ItemData == null) return false;
        if (!IsPrefab()) return false;
        return ItemData.Prefab == null;
    }

    private bool CanCreateItemData()
    {
        if (ItemData != null) return false;
        return IsPrefab();
    }

    private bool IsPrefab()
    {
        return PrefabUtility.IsPartOfPrefabAsset(gameObject) || PrefabStageUtility.GetPrefabStage(gameObject) != null;
    }
    #endregion
#endif
}
