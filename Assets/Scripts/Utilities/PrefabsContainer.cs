using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs Container", menuName = "ScriptableObjects/Items/PrefabsContainer")]
public class PrefabsContainer : ScriptableObject
{
    [SerializeField] List<ItemData> _prefabs = new List<ItemData>();

    public GameObject GetPrefab(int id)
    {
        if(id < 0)
        {
            Debug.LogError($"Index out of range - {id}");
            return null;
        }

        if(id >= _prefabs.Count)
        {
            Debug.LogError($"Index out of range - {id}");
            return null;
        }

        return _prefabs[id].Prefab.gameObject;
    }

    public int GetNextIDLooped(int id) => (id + 1) % _prefabs.Count;
    public int GetNextID(int id) =>Mathf.Clamp(id + 1, 0, _prefabs.Count - 1);
    public bool IsHighestID(int id) => id + 1 >= _prefabs.Count;
}
