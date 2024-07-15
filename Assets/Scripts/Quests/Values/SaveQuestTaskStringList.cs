using System.Collections.Generic;

[System.Serializable]
public class SaveQuestTaskStringList : ASaveQuestTask
{
    public List<string> Value;

    public SaveQuestTaskStringList(List<string> value)
    {
        Value = new(value);
    }
}
