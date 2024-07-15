[System.Serializable]
public class SaveQuestTaskBoolValue : ASaveQuestTask
{
    public bool Value;

    public SaveQuestTaskBoolValue(bool value)
    {
        Value = value;
    }
}
