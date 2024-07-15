using Sirenix.OdinInspector;
using UnityEngine;


public abstract class Achievement : ScriptableObject
{
    [SerializeField] protected SteamCommunicator _steamCommunicator;

    public string _steamName;
    [field: SerializeField] public ActionStat ActionStat { get; private set; }

    public virtual bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if (executedActionStat == null) return false;
        if (ActionStat == null) return false;
        return executedActionStat.Guid == ActionStat.Guid;
    }

    [Button("SetAchievement")]
    private void TestPerform()
    {
        TryPerform(ActionStat);
    }
}
