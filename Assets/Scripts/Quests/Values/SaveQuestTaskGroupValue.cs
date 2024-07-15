using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveQuestTaskGroupValue : ASaveQuestTask
{
    [SerializeReference] public List<ASaveQuestTask> Value = new();

    public SaveQuestTaskGroupValue(List<ASaveQuestTask> value)
    {
        Value = new(value);
    }
}
