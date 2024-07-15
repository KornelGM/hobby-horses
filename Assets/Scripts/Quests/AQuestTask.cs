using UnityEngine;
using I2.Loc;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;

public abstract class AQuestTask : ScriptableObject
{
    public event Action<AQuestTask> OnTaskProgress;

    [SerializeField, BoxGroup("Task Stat", centerLabel: true)] protected ActionStat _usedActionStat;
    [SerializeField, FoldoutGroup("Localized settings")] protected LocalizedString _description;
    [SerializeField, FoldoutGroup("Localized settings")] protected LocalizedString _helpText;

    [SerializeField, FoldoutGroup("Finder settings")] public AQuestTargetFinder QuestTargetFinder;
    [SerializeField, FoldoutGroup("Finder settings")] public bool QuestFinderCanBeNull;
    [SerializeField, FoldoutGroup("Finder settings")] public bool RefreshFinderPositionOnTaskUpdate;

    [SerializeField, FoldoutGroup("Task Reward")] public List<AQuestReward> TaskRewards;
    [SerializeField, FoldoutGroup("Additive settings")] protected bool _searchForAlreadyDoneActions;

    public bool IsCompleted { get; protected set; } = false;

    public virtual void ForceCompleteTask()
    {
        IsCompleted = true;
    }

    public virtual void PrepareTask(Action<AQuestTask> OnTaskProgress) =>
        this.OnTaskProgress += OnTaskProgress;

    public abstract bool TryCompleteTask(ActionStat actionStat = null);

    public virtual LocalizedString GetQuestTaskDescription() =>
        _description;

    public virtual LocalizedString GetQuestTaskHelp() =>
        _helpText;
    public virtual bool CompareActionStatData(AActionStatData data = null) { return true; }
    private bool CompareActionStatData(List<AActionStatData> datas = null)
    {
        foreach (AActionStatData statData in datas)
        {
            if (CompareActionStatData(statData)) return true;
        }
        return false;
    }
    public virtual string GetCounter() => "";

    public abstract void Initialize(ASaveQuestTask saveQuestTask);
    public virtual void Initialize() { }
    public abstract ASaveQuestTask GetQuestTaskSaveData();

    public bool ActionHasAlreadyBeenDone()
    {
        if (!_searchForAlreadyDoneActions) return false;
        SceneServiceLocator serviceLocator = SceneServiceLocator.Instance;
        if (serviceLocator == null) return false;

        if (serviceLocator.TryGetServiceLocatorComponent(out StatsManager statManager))
        {
            if (!statManager.Statistics.ContainsKey(_usedActionStat.Guid)) return false;
            return CompareActionStatData(statManager.Statistics[_usedActionStat.Guid]);
        }
        return false;
    }

    public IEnumerator AwardPlayer(MonoBehaviour emptyMonoBehaviour)
    {
        foreach (AQuestReward qr in TaskRewards)
        {
            yield return emptyMonoBehaviour.StartCoroutine(qr.AwardPlayer());
        }
    }

    public void InvokeOnTaskProgress() =>
        OnTaskProgress?.Invoke(this);

    public void ClearOnTaskProgress() =>
        OnTaskProgress = null;
}
