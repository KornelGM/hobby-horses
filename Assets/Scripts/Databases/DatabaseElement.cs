using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract class DatabaseElement<T> : SerializedScriptableObject, IGetDatabaseElement where T: Object
{
    [field: SerializeField] public virtual List<T> EntryList { get; set; } = new();
    [field: SerializeField, ReadOnly] public List<string> GuidsList { get; set; } = new();
    [field: OdinSerialize, HideInInspector] public Dictionary<string, T> Entries { get; private set; } = new();
    [field: SerializeField, ReadOnly] public string Guid { get; set; }

    [Button("Log Path")]
    public void WritePath()
    {
        foreach (string entry in GetPath())
        {
            Debug.Log(entry);
        }
    }

    public abstract List<string> GetPath();

    public string GetGuid() => Guid;

    public string GetGuidOfElement(T element)
    {
        int findIndex = EntryList.FindIndex(x => (dynamic)x == element);
        if (findIndex < 0 || findIndex >= GuidsList.Count) return null;
        return GuidsList[findIndex];
    }

    public string GetGuidOfElement(System.Predicate<T> predicate)
    {
        int findIndex = EntryList.FindIndex(predicate);
        return GuidsList[findIndex];
    }

    public virtual N GetElementOfPath<N>(List<string> path) where N : Object
    {
        if (path == null) {
            Debug.LogError("Path is null", this);
            return null;
        }

        if (path.Count == 0)
        {
            Debug.LogError("Path is empty", this);
            return null;
        }

        string elementGuid = path[0];
        T entry = GetEntry(elementGuid);
        path.RemoveAt(0);

        if(Utilities.TryCast(entry, out N element)) return element; //if element has nested database element and its not of type we're looking for 
        if(Utilities.TryCast(entry, out IGetDatabaseElement pathable)) return pathable.GetElementOfPath<N>(path); //if element is of type we're looking for
        
        Debug.LogError($"Searched database element of GUID: {elementGuid} and type: {typeof(N)} was not found", this);
        return null;
    }

    public void SetEntries(Dictionary<string, T> newEntries)
    {
        Entries = newEntries;
    }

    public T GetEntry()
    {
        if (EntryList.Count == 0)
        {
            Debug.LogError($"The database element ({this.name}) is empty, please check it out", this);
            return default;
        }

        T dbElment = EntryList[Random.Range(0, EntryList.Count)];

        if(dbElment == null)
        {
            Debug.LogError($"The element of database was not found in {this.name}", this);
            return default;
        }

        return dbElment;
    }

    public T GetEntry(string guid)
    {
        T dbElement;

        try
        {
            dbElement = Entries[guid];
            return dbElement;
        }
        catch
        {
            Debug.LogError($"The database element of GUID: {guid} in {this.name} was not found", this);
            return GetEntry();
        }
    }

    public T GetEntryOrDefault(string guid)
    {
        T dbElement;

        try
        {
            dbElement = Entries[guid];
            return dbElement;
        }
        catch
        {
            Debug.LogError($"The database element of GUID: {guid} in {this.name} was not found", this);
            return default;
        }
    }

    public T GetNextEntry(string guid) => GetNextEntry(GetEntry(guid));

    public T GetNextEntry(T element)
    {
        int index = EntryList.IndexOf(element);
        if (EntryList.Count > index + 1) 
            return EntryList[index + 1];

        return EntryList[0];
    }

    public T GetPreviousEntry(string guid) => GetNextEntry(GetEntry(guid));

    public T GetPreviousEntry(T element)
    {
        int index = EntryList.IndexOf(element);
        if (index == 0)
            return EntryList[^1];

        return EntryList[index -1];
    }

    protected virtual List<string> GetPath<N>(DatabaseElement<N> parent) where N: Object
    {
        List<string> path = parent.GetPath();
        path.Add(GetGuid());
        return path;
    }

    public int GetIndexOf(T element)
    {
        if (element == null) return -1;

        return EntryList.IndexOf(element);
    }

    public int GetIndexOf(System.Predicate<T> predicate)
    {
        return EntryList.FindIndex(predicate);
    }

    public int GetIndexOf(string guid)
    {
        return GetIndexOf(GetEntryOrDefault(guid));
    }

    public T GetElementOfIndex(int index)
    {
        if (index < 0 || index >= EntryList.Count) return default;
        return EntryList[index];
    }

#if UNITY_EDITOR
    [Button, InfoBox("Use with caution",InfoMessageType.Warning)]
    private void GetAllItems()
    {
        string typeName = typeof(T).ToString();    
        var list = AssetDatabase.FindAssets($"t:{typeName}").Select(x => AssetDatabase.GUIDToAssetPath(x))
        .Select(x => AssetDatabase.LoadAssetAtPath(x,typeof(UnityEngine.Object))).ToList();
        List<T> newEntries = new(); 
        foreach(var obj in list)
        {
            if(!Utilities.TryCast(obj, out T result))continue;

            newEntries.Add(result);
        }
        EntryList = newEntries;

        Debug.Log("Successfuly implemented all datas to database");
    }

    public virtual void RefreshDictionary()
    {
        Guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        List<string> guids = new();
        foreach (T entry in EntryList) 
        {
            guids.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(entry)));
            if (Utilities.TryCast(entry, out IParent<T> parentObject))
            {
                parentObject.Parent = this;
            }
        }

        GuidsList = guids;

        Dictionary<string, T> newDict = new();

        for (int i = EntryList.Count-1; i >= 0; i--)
        {
            if (EntryList[i] == null)
            {
                EntryList.RemoveAt(i);
                continue;
            }
            newDict.Add(GuidsList[i], EntryList[i]);
        }
        SetEntries(newDict);

        EditorUtility.SetDirty(this);
    }

#endif
}

public interface IParent<T> where T: Object
{
    public DatabaseElement<T> Parent { get; set; }
}

public interface IGetDatabaseElement
{
    public N GetElementOfPath<N>(List<string> path) where N : Object;
}
