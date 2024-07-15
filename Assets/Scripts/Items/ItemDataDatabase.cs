using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR    
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item Data Database", menuName = "ScriptableObjects/Items/Item Data Database")]
public class ItemDataDatabase : DatabaseElement<ItemData>
{
#if UNITY_EDITOR

    [Button("Find doubled prefabs")]
    private void FindDoubledPrefabs()
    {
        Dictionary<GameObject, List<ItemData>> datas = Entries.
            Where(item => item.Value?.Prefab != null).
            GroupBy(group => group.Value?.Prefab).
            Where(group => group.Count() > 1).
            ToDictionary(itemData => itemData.First().Value?.Prefab?.gameObject,
            itemData => itemData.Select(item => item.Value).ToList());

        StringBuilder txt = new();
        foreach (var data in datas)
        {
            txt.Clear();
            txt.Append($"There is doubled data with prefab {data.Key.name}:");
            foreach (ItemData itemData in data.Value)
            {
                txt.Append($"\n- {itemData.name}");
            }
            Debug.LogWarning(txt);
        }
    }

    [Button("Find missing prefabs")]
    private void FindMissingPrefabs()
    {
        List<ItemData> datas = Entries.
            Where(item => item.Value?.Prefab == null).
            Select(data => data.Value).ToList();

        foreach (var data in datas)
        {
            Debug.LogWarning($"There is missing prefab inside {data.name}", data);
        }
    }

    [Button("Try Inject ItemData\nto its prefabs", 50)]
    private void TryInjectItemDatas()
    {
        foreach (ItemData data in Entries.Values)
        {
            if (data == null)
            {
                Debug.LogWarning("There is missing data");
                continue;
            }

            if (data.Prefab == null)
            {
                Debug.LogWarning($"There is missing prefab inside {data.name}", data);
                continue;
            }

            if (data.Prefab.TryGetComponent(out ItemDataContainer dataContainer))
            {
                dataContainer.ItemData = data;
                EditorUtility.SetDirty(data.Prefab);
                EditorUtility.SetDirty(dataContainer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(data.Prefab);

                //AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
            }
        }
    }


    public ItemData GetItemData(ServiceLocator prefab)
    {
        foreach (var data in Entries.Values)
        {
            if (data == null) continue;
            if (data.Prefab == prefab) return data;
        }
        return null;
    }
#endif

    public override List<string> GetPath()
    {
        Debug.Log("Log path not available in this script");
        return new();
    }

}
