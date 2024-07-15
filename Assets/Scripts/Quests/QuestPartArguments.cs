using System;
using I2.Loc;

[Serializable]
public class QuestPartArguments
{
    public LocalizedString Description;
    public TransformVariable TransformToSelect;

    public int[] IntArguments;
    public float[] FloatArguments;
    public string[] StringArguments;
    public UnityEngine.Object[] ObjectArguments;
}
