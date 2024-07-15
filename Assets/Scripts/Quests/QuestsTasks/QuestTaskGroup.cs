using System;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[CreateAssetMenu(fileName = "QuestTask Group", menuName = "ScriptableObjects/Quests/Tasks/Group")]
public class QuestTaskGroup : AQuestTask
{
    [SerializeField] List<AQuestTask> _tasks;
    [SerializeField] int _requiredFinishedTasks;
    [SerializeField] bool _showCounter;

    [HideInInspector] public List<AQuestTask> Tasks;

    private int _currentlyFinished = 0;

    public override void PrepareTask(Action<AQuestTask> OnTaskProgress)
    {
        base.PrepareTask(OnTaskProgress);

        foreach (AQuestTask qt in _tasks)
        {
            AQuestTask task = Instantiate(qt);
            task.PrepareTask(OnTaskProgress);
            Tasks.Add(task);
        }
    }

    public override bool TryCompleteTask(ActionStat actionStat = null)
    {
        foreach (AQuestTask qt in Tasks)
        {
            if (qt.IsCompleted)
                continue;

            if (qt.TryCompleteTask(actionStat))
            {
                ++_currentlyFinished;
                InvokeOnTaskProgress();
                qt.InvokeOnTaskProgress();
            }
        }

        if (_currentlyFinished >= _requiredFinishedTasks)
        {
            ClearOnTaskProgress();
            IsCompleted = true;
        }

        return IsCompleted;
    }

    public override string GetCounter()
    {
        return _showCounter ? $"({_currentlyFinished}/{_requiredFinishedTasks})" : "";
    }

    public override void Initialize(ASaveQuestTask saveQuestTask)
    {
        SaveQuestTaskGroupValue value = saveQuestTask as SaveQuestTaskGroupValue;
        
        for (int i = 0; i < Tasks.Count; ++i)
        {
            Tasks[i].Initialize(value.Value[i]);

            if (Tasks[i].TryCompleteTask())
                ++_currentlyFinished;
        }
    }

    public override ASaveQuestTask GetQuestTaskSaveData()
    {
        List<ASaveQuestTask> saveForTasks = new();

        foreach (AQuestTask task in Tasks)
            saveForTasks.Add(task.GetQuestTaskSaveData());

        return new SaveQuestTaskGroupValue(saveForTasks);
    }
}
