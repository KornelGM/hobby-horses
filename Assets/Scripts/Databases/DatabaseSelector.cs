using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseSelector<T1, T2> : MonoBehaviour where T1 : Object
{
    public DatabaseElement<T1> Database;
    public List<string> ActiveIds = new(); 
    public List<IdToObjectPair<T2>> ActivePairs = new();
    protected Dictionary<string, T2> activeDict = new();

    public void GenerateDictionary()
    {
        activeDict = new();

        foreach (IdToObjectPair<T2> pair in ActivePairs)
        {
            activeDict.Add(pair.Id, pair.Obj);
        }
    }
}