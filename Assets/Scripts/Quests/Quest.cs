using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest")]
public class Quest : ScriptableObject
{
    public event Action<Quest, AQuestTask> OnQuestTaskCompleted;
    public event Action<Quest, AQuestTask> OnQuestTaskProgress;

    public LocalizedString Title;
    public List<Quest> NextQuests = new();
    [field: SerializeField] public PadlockList ConditionalLocks = new();
    [SerializeField] List<AQuestActivationBehaviour> _questActivationBehaviours;
    [SerializeField] List<AQuestTask> _questTasks;
    [SerializeField] List<AQuestReward> _questRewards;

    public bool IsQuestActive { get; set; } = false;

    [SerializeField, ReadOnly] public List<AQuestTask> QuestTasks = new();
    [SerializeField, ReadOnly] public List<AQuestReward> QuestRewards = new();

    private bool _activated = false;

    [Button("Clear quest")]
    public void ClearQuests()
    {
        //Debug.Log($"Clear quest \"{Title}\"...");
        QuestTasks = new();
        QuestRewards = new();
    }

    public void StartQuest(MonoBehaviour monoBehaviour)
    {
        bool logging = false;
        if (SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out DreamParableLogger.Logger logger ))
        {
            logger.Log(LogType.Log, "Quest", this, $"Starting... {Title}");
            logging = true;
        }

        foreach (AQuestTask qt in _questTasks)
        {
            if(logging)
                logger.Log(LogType.Log, "Quest", this, $"Trying add new task... {qt.name}");

            AQuestTask questTask = Instantiate(qt);
            questTask.PrepareTask(QuestTaskProgress);
            QuestTasks.Add(questTask);

            if (logging)
                logger.Log(LogType.Log, "Quest", this, $"Task {qt.name} added.");
        }

        foreach (AQuestReward qr in _questRewards)
        {
            if (logging)
                logger.Log(LogType.Log, "Quest", this, $"Trying add new reward... {qr.name}");

            AQuestReward questReward = Instantiate(qr);
            QuestRewards.Add(questReward);

            if (logging)
                logger.Log(LogType.Log, "Quest", this, $"Reward {qr.name} added.");
        }

        Initialize();
        CheckIfTasksHasAlreadyBeenDone(monoBehaviour);
    }

    public bool IsQuestCompleted(ActionStat actionStat)
    {
        bool isQuestCompleted = true;

        foreach (AQuestTask qt in QuestTasks)
        {
            if (qt == null) return true;
            if (qt.IsCompleted)
                continue;

            bool taskCompleted = qt.TryCompleteTask(actionStat);

            if (!taskCompleted)
                isQuestCompleted = false;
            else if (taskCompleted)
                OnQuestTaskCompleted?.Invoke(this, qt);
        }

        return isQuestCompleted;
    }

    public void CompleteNextTask()
    {
        foreach (AQuestTask qt in QuestTasks)
        {
            if (qt == null) continue;
            if (qt.IsCompleted)
                continue;

            qt.ForceCompleteTask();
            OnQuestTaskCompleted?.Invoke(this, qt);
            return;
        }
    }

    private void QuestTaskProgress(AQuestTask questTask)
    {
        OnQuestTaskProgress?.Invoke(this, questTask);
    }

    public void Initialize(SaveQuest saveQuest)
    {
        _activated = saveQuest.Activated;

        for (int i = 0; i < QuestTasks.Count; ++i)
            QuestTasks[i].Initialize(saveQuest.QuestTasks[i]);

        IsQuestCompleted(null);
    }

    private void Initialize()
    {
        for (int i = 0; i < QuestTasks.Count; ++i)
        {
            if (QuestTasks[i] == null)
                continue;

            QuestTasks[i].Initialize();
        }
    }

    public SaveQuest GetQuestSaveData()
    {
        SaveQuest saveQuest = new();

        saveQuest.Activated = _activated;

        foreach (AQuestTask qt in QuestTasks)
            saveQuest.QuestTasks.Add(qt.GetQuestTaskSaveData());

        return saveQuest;
    }

    public IEnumerator AwardPlayer(MonoBehaviour emptyMonoBehaviour)
    {
        foreach (AQuestReward qr in QuestRewards)
        {
            if (qr == null)
                continue;

            yield return emptyMonoBehaviour.StartCoroutine(qr.AwardPlayer());
        }
    }

    public IEnumerator Activate(MonoBehaviour emptyMonoBehaviour)
    {
        if (_activated)
            yield break;

        _activated = true;

        foreach (AQuestActivationBehaviour activationBehaviour in _questActivationBehaviours)
        {
#if UNITY_EDITOR
            if (activationBehaviour.RunInEditor)
                yield return emptyMonoBehaviour.StartCoroutine(activationBehaviour.ExecuteBehaviour());
#else
            yield return emptyMonoBehaviour.StartCoroutine(activationBehaviour.ExecuteBehaviour());
#endif
        }
    }

    public void CheckIfTasksHasAlreadyBeenDone(MonoBehaviour monoBehaviour)
    {
        foreach (AQuestTask qt in QuestTasks)
        {
            if (qt == null)
                continue;

            if (qt.ActionHasAlreadyBeenDone())
            {
                qt.ForceCompleteTask();
                OnQuestTaskCompleted?.Invoke(this, qt);
                monoBehaviour.StartCoroutine(qt.AwardPlayer(monoBehaviour));
            }
        }
    }

    public void Reset()
    {
        IsQuestActive = false;
        OnQuestTaskCompleted = null;
        QuestTasks = new();
        QuestRewards = new();

        _activated = false;
    }
}
