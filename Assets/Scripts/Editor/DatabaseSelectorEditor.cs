#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatabaseSelector<,>), true)]
public class DatabaseSelectorEditor<T1, T2> : OdinEditor where T1 : DatabaseElement<T2> where T2: Object
{
    bool selectElementsOpen;

    public override void OnInspectorGUI()
    {
        DatabaseSelector<T1, T2> selector = target as DatabaseSelector<T1, T2>;
        selector.Database = EditorGUILayout.ObjectField("Database", selector.Database, typeof(DatabaseElementWithClassGenerator<T1>), false) as DatabaseElementWithClassGenerator<T1>;

        if (!selector.Database)
        {
            return;
        }

        List<string> activeSounds;

        selectElementsOpen = EditorGUILayout.Foldout(selectElementsOpen, new GUIContent("Selected elements"));
        if (selectElementsOpen)
        {
            activeSounds = new();
            List<T1> entries = selector.Database.EntryList;
            EditorGUI.indentLevel++;

            foreach (T1 element in entries)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(element));
                bool selected = selector.ActiveIds.Contains(guid);

                selected = EditorGUILayout.Toggle(element.name, selected);

                IdToObjectPair<T2> pair = selector.ActivePairs.FirstOrDefault(p => p.Id == guid);

                if (selected)
                {
                    activeSounds.Add(guid);
                }
                else if (pair != null)
                {
                    selector.ActivePairs.Remove(pair);
                }
            }
            EditorGUI.indentLevel--;
            selector.ActiveIds = activeSounds;
        }

        activeSounds = new();

        foreach (string id in selector.ActiveIds)
        {
            List<T1> entries = selector.Database.EntryList;
            DatabaseElement<T2> element = entries.FirstOrDefault(e => AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(e)).ToString() == id);

            if (element == null)
            {
                continue;
            }

            activeSounds.Add(id);

            IdToObjectPair<T2> pair = selector.ActivePairs.FirstOrDefault(p => p.Id == id);

            List<T2> innerElements = element.EntryList;

            int index = 0;
            if (pair != null)
            {
                index = innerElements.FindIndex(v => v == pair.Obj);
            }

            index = EditorGUILayout.Popup(element.name, index, innerElements.Select(v => v.name).ToArray());

            if (pair != null)
            {
                pair.Obj = innerElements[index];
            }
            else
            {
                selector.ActivePairs.Add(new IdToObjectPair<T2>(id, innerElements[index]));
            }
        }

        selector.ActiveIds = activeSounds;

        if (Application.isPlaying)
        {
            selector.GenerateDictionary();
        }
    }
}
#endif