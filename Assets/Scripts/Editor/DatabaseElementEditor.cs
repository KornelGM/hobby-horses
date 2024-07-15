using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(DatabaseElement<>), true)]
[CanEditMultipleObjects]

public class DatabaseElementEditor<T> : OdinEditor where T : Object
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SaveDict();
    }

    protected virtual void OnDestroy()
    {
        CheckIfElementsAreNotNull();
        SaveDict();
    }

    protected void CheckIfElementsAreNotNull()
    {
        DatabaseElement<T> e = target as DatabaseElement<T>;

        List<T> newEntries = new();
        List<T> currentEntries = e.EntryList;
        List<string> newGuids = new();
        List<string> currentGuids = e.GuidsList;

        for (int i = 0; i < currentEntries.Count; ++i)
        {
            if (currentEntries[i] == null)
            {
                Debug.LogWarning($"Element with index = {i} was null, the value was removed");
                continue;
            }

            newEntries.Add(currentEntries[i]);
            newGuids.Add(currentGuids[i]);
        }

        e.EntryList = newEntries;
        e.GuidsList = newGuids;
    }

    protected void SaveDict()
    {
        DatabaseElement<T> dbElement = target as DatabaseElement<T>;
        if (dbElement == null) return;
        dbElement.RefreshDictionary();
    }
}
