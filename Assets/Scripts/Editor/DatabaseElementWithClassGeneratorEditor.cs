#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DatabaseElementWithClassGenerator<>), true)]
[CanEditMultipleObjects]
public class DatabaseElementWithClassGeneratorEditor<T> : DatabaseElementEditor<T> where T : Object
{
    protected virtual string Postfix { get; set; } = "Top";
    protected virtual string TopNameSubfix { get; set; } = "TopElement";

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GenerateStaticClass();
    }

    private void GenerateStaticClass()
    {
        
        Debug.Log($"Generating {target.name}{Postfix}");
        DatabaseElement<T> db = target as DatabaseElement<T>;
        StringBuilder builder = new();
        builder.Append("// THIS CLASS WAS GENERATED AUTOMATICALLY\n");
        builder.Append("// DON'T CHANGE IT MANUALLY AS ALL CHANGES WILL BE OVERWRITTEN\n\n");
        builder.Append($"public static class {target.name}{Postfix}\n");
        builder.Append("{\n");
        List<string> names = new();
        List<string> guids = new();
        List<T> entries = db.EntryList;

        foreach (T top in entries)
        {
            if (top == null)
            {
                Debug.LogError("One of the events is null. Execution was aborted");
            }

            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(top));
            string name;
            if (top.name.StartsWith(TopNameSubfix))
            {
                name = top.name.Replace(TopNameSubfix, "").Trim('_').Trim().ToUpper().Replace(" ", "_");
            }
            else
            {
                name = top.name.Trim('_').Trim().ToUpper().Replace(" ", "_");
            }

            if (names.Contains(name))
            {
                throw new System.Exception($"Object {top} duplicates name for value {name}");
            }


            if (guids.Contains(guid))
            {
                throw new System.Exception($"Object {top} is duplicated");
            }

            if (top is DatabaseElement<T>)
            {
                CheckInnerObjects(top as DatabaseElement<T>, $"{top}");
            }

            names.Add(name);
            guids.Add(guid);
            builder.Append($"    public static readonly string {name} = \"{guid}\";\n");
        }
        builder.Append("}\n");
        string path = $"{Path.GetDirectoryName($"Assets\\Scripts\\Auto-generated\\Scripts{Postfix}")}";
        string filePath = $"{path}\\{target.name}{Postfix}.cs";
        string newText = builder.ToString();

        if (File.Exists(filePath) && File.ReadAllText(filePath) == newText)
        {
            Debug.Log($"File {filePath} content is the same, exiting...");
            return;
        }

        DatabaseStaticClassWriter.ShowWindow(filePath, builder.ToString());
        
    }

    private void CheckInnerObjects(DatabaseElement<T> db, string parentString)
    {
        List<T> entries = db.EntryList;
        foreach (T obj in entries)
        {
            if (obj == null)
            {
                throw new System.Exception($"One of the elements of {parentString} is null. Execution was aborted");
            }

            if (obj is DatabaseElement<T>)
            {
                CheckInnerObjects(obj as DatabaseElement<T>, $"{parentString} > {obj}");
            }
        }
    }
}
#endif