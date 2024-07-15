using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quests Database", menuName = "ScriptableObjects/Quests/Quests Database")]
public class QuestsDatabase : DatabaseElement<Quest>
{
    public override List<string> GetPath() => new();

    [Button("Clear All Quests")]
    public void ClearQuests()
    {
        foreach (var item in EntryList)
        {
            item.ClearQuests();
        }
        Debug.Log("Cleared all quests");
    }
}
