using UnityEngine;
using I2.Loc;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestTask Reputation Level", menuName = "ScriptableObjects/Quests/Tasks/Reputation Level")]
public class QuestTaskReputationLevel : AQuestTask
{
    [SerializeField, FoldoutGroup("Specific Settings")] private int _needReputationLevel;
    [SerializeField, FoldoutGroup("Specific Settings")] private bool _showNeedLevel;

    private bool _complete;

    public override void ForceCompleteTask()
    {
        InvokeOnTaskProgress();
        base.ForceCompleteTask();
    }

    public override bool TryCompleteTask(ActionStat actionStat = null)
    {
        if (actionStat == null) return false;

        if (actionStat.Guid == _usedActionStat.Guid)
        {
            if (!CompareActionStatData(actionStat.Data))
                return false;

            if (_complete)
            {
                ClearOnTaskProgress();
                IsCompleted = true;
            }
        }
        return IsCompleted;
    }

    public override string GetCounter()
    {
        return _showNeedLevel ? $"[{_needReputationLevel}]" : "";
    }

    public override bool CompareActionStatData(AActionStatData data = null)
    {
        IntActionStatData functionData = data as IntActionStatData;

        if (functionData == null)
            return false;

        if (_needReputationLevel > functionData.Value) 
            return false;

        _complete = true;
        InvokeOnTaskProgress();

        return _complete;
    }     

    public override void Initialize(ASaveQuestTask saveQuestTask)
    {
        SaveQuestTaskBoolValue saveQuest = saveQuestTask as SaveQuestTaskBoolValue;

        _complete = saveQuest.Value;

        IsCompleted = _complete;
    }

    public override ASaveQuestTask GetQuestTaskSaveData()
    {
        return new SaveQuestTaskBoolValue(_complete);
    }
}
