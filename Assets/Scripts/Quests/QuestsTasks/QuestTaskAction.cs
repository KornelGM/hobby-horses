using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestTask Action", menuName = "ScriptableObjects/Quests/Tasks/Action")]
public class QuestTaskAction : AQuestTask
{
    [SerializeField, FoldoutGroup("Specific Settings")] private bool _showCounter;
    [SerializeField, FoldoutGroup("Specific Settings")] private int _amount = 1;
    bool _complete = false;

    private int _count = 0;

    public override void ForceCompleteTask()
    {
        _complete = true;
        _count = _amount;
        InvokeOnTaskProgress();
        base.ForceCompleteTask();
    }

    public override bool TryCompleteTask(ActionStat actionStat = null)
    {
        if (actionStat == null) return IsCompleted;

        if (actionStat.Guid == _usedActionStat.Guid)
        {
            _count++;
            _complete = _count >= _amount;

            InvokeOnTaskProgress();

            if (_complete)
            {
                ClearOnTaskProgress();
                IsCompleted = true;
            }
        }
        return IsCompleted;
    }

    public override void Initialize(ASaveQuestTask saveQuestTask)
    {
        SaveQuestTaskGroupValue saveData = saveQuestTask as SaveQuestTaskGroupValue;

        if (saveData == null || saveData.Value is not { Count: > 0 })
            return;

        foreach (var data in saveData.Value)
        {
            if (data is SaveQuestTaskBoolValue)
            {
                _complete = (data as SaveQuestTaskBoolValue).Value;
                IsCompleted = _complete;
                continue;
            }
            else if (data is SaveQuestTaskIntValue)
            {
                _count = (data as SaveQuestTaskIntValue).Value;
                continue;
            }
        }
    }

    public override string GetCounter()
    {
        return _showCounter ? $"({_count}/{_amount})" : "";
    }

    public override ASaveQuestTask GetQuestTaskSaveData()
    {
        List<ASaveQuestTask> saveData = new()
        {
            new SaveQuestTaskBoolValue(_complete),
            new SaveQuestTaskIntValue(_count)
        };

        return new SaveQuestTaskGroupValue(saveData);
    }
}