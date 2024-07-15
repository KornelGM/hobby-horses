using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IServiceLocatorComponent, IAwake, IStartable, IUpdateable, IManager, IQuestManager, ISaveable<SaveData>
{
    [ServiceLocatorComponent] private StatsManager _actionStatsManager;
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;

    [SerializeField] private QuestWaypoint _missionWaypointPrefab;
    [SerializeField] private DatabaseElement<Quest> _database;
    [SerializeField] private QuestUIController _questPanelPrefab;
    [SerializeField] private Quest _startQuest;

    private QuestUIController _createdQuestPanel;

    public bool Enabled => true;

    public ServiceLocator MyServiceLocator { get; set; }

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestActivated;
    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest, AQuestTask> OnQuestTaskCompleted;
    public event Action<Quest, AQuestTask> OnQuestTaskProgressed;

    private QuestWaypoint _missionWaypoint;
    private List<Quest> _questsInProgress = new();
    private List<Quest> _questsCompleted = new();
    private Quest _questActive;
    
    private Transform _transformToFollow;

    public void CustomReset()
    {
        OnQuestStarted = null;
        OnQuestActivated = null;
        OnQuestCompleted = null;
        OnQuestTaskCompleted = null;
        OnQuestTaskProgressed = null;

        _questsInProgress.ForEach(q => q.Reset());
        _questsCompleted.ForEach(q => q.Reset());

        _questsInProgress = new();
        _questsCompleted = new();
        _questActive = null;
    }

    public void CustomAwake()
    {
        CustomReset();
        Configure();
    }

    public void Configure()
    {
        _missionWaypoint?.gameObject.SetActive(false);
        _actionStatsManager.OnStatisticAdded += CheckQuestsInProgress;
    }

    public void CustomStart()
    {
        _createdQuestPanel = _windowManager.CreateWindow(_questPanelPrefab).GetComponent<QuestUIController>();
    }

    private void OnDestroy()
    {
        _actionStatsManager.OnStatisticAdded -= CheckQuestsInProgress;
    }

    public void CustomUpdate()
    {
        if (_transformToFollow != null && _missionWaypoint != null)
            _missionWaypoint.Target.position = _transformToFollow.position;
    }

    public void ToggleQuestPanel(bool toogle)
    {
        if (_createdQuestPanel == null) return;

        _createdQuestPanel.transform.localScale = toogle ? Vector3.one : Vector3.zero;
    }

    public void Initialize(SaveData save)
    {       
        if(_database is QuestsDatabase questsDatabase)
        {
            questsDatabase.ClearQuests();
        }

        if (save == null)
        {
            AddQuest(_startQuest, true);
            return;
        }

        if ( save.PlayerSaveData == null || save.PlayerSaveData.Quests == null)
            return;


        foreach (SaveQuest saveQuest in save.PlayerSaveData.Quests.InProgressQuests)
        {
            Quest quest = _database.GetEntryOrDefault(saveQuest.Guid);

            if (quest == null)
                continue;

            bool isCurrentQuest = save.PlayerSaveData.Quests.ActiveQuestGuid == saveQuest.Guid;

            if (!_questsInProgress.Contains(quest))
                AddQuest(quest, isCurrentQuest);

            quest.Initialize(saveQuest);

            if(isCurrentQuest)
                SetQuestIndicator(quest);
        }

        foreach (SaveQuest saveQuest in save.PlayerSaveData.Quests.CompletedQuests)
        {
            Quest quest = _database.GetEntryOrDefault(saveQuest.Guid);

            if (quest == null)
                continue;

            quest.Initialize(saveQuest);

            if (!_questsCompleted.Contains(quest))
                _questsCompleted.Add(quest);
        }
    }

    public SaveData CollectData(SaveData data)
    {
        QuestsSaveData questsSaveData = new();

        foreach (Quest quest in _questsInProgress)
        {
            SaveQuest saveQuest = quest.GetQuestSaveData();
            saveQuest.Guid = _database.GetGuidOfElement(quest);
            questsSaveData.InProgressQuests.Add(saveQuest);

            if (quest == _questActive)
                questsSaveData.ActiveQuestGuid = saveQuest.Guid;
        }

        foreach (Quest quest in _questsCompleted)
        {
            SaveQuest saveQuest = quest.GetQuestSaveData();
            saveQuest.Guid = _database.GetGuidOfElement(quest);
            questsSaveData.CompletedQuests.Add(saveQuest);
        }

        data.PlayerSaveData.Quests = questsSaveData;
        return data;
    }

    public void AddQuest(Quest quest, bool setQuestActive = false)
    {
        if (!_database.EntryList.Contains(quest))
        {
            _logger.Log(LogType.Warning, "Quest Manager", this, $"Quest \"{quest.Title}\" is not in entry list of the manager");
            return;
        }

        if (_questsInProgress.Contains(quest) || _questsCompleted.Contains(quest))
        {
            _logger.Log(LogType.Warning, "Quest Manager", this, $"Quest \"{ quest.Title}\" already started");
            return;
        }

        _logger.Log(LogType.Log, "Quest Manager", this, $"Trying add new quest... {quest.name}");

        quest.StartQuest(this);

        quest.OnQuestTaskCompleted += QuestTaskCompleted;
        _questsInProgress.Add(quest);
        OnQuestStarted?.Invoke(quest);

        _logger.Log(LogType.Log, "Quest Manager", this, $"Quest {quest.Title} has been added");

        if (setQuestActive)
        {
            _logger.Log(LogType.Log, "Quest Manager", this, $"Activate quest {quest.Title}");
            ActivateQuest(quest);
        }

        CheckQuestsInProgress(null);
    }

    public void ActivateQuest(Quest quest)
    {
        if (!_questsInProgress.Contains(quest))
        {
            _logger.Log(LogType.Warning, "Quest Manager", this, $"Quest \"{quest}\" is not yet started");
            return;
        }

        if (_questActive != null)
        {
            _questActive.IsQuestActive = false;
            _questActive.OnQuestTaskProgress -= QuestTaskProgress;
        }

        _questActive = quest;
        _questActive.IsQuestActive = true;
        _questActive.OnQuestTaskProgress += QuestTaskProgress;
        StartCoroutine(_questActive.Activate(this));

        OnQuestActivated?.Invoke(quest);

        SetQuestIndicator(quest);
    }

    private void CheckQuestsInProgress(ActionStat actionStat)
    {
        StartCoroutine(CheckQuestsInProgressCoroutine(actionStat));
    }

    private IEnumerator CheckQuestsInProgressCoroutine(ActionStat actionStat)
    {
        for (int i = _questsInProgress.Count - 1; i >= 0; --i)
        {
            Quest q = _questsInProgress[i];

            if (_questsCompleted.Contains(q))
                continue;

            if (!q.IsQuestCompleted(actionStat))
                continue;

            _questsCompleted.Add(q);
            OnQuestCompleted?.Invoke(q);

            yield return StartCoroutine(q.AwardPlayer(this));

            _logger.Log(LogType.Log, "Quest Manager", this, $"Completed quest {q.Title}");
            _questsInProgress.RemoveAt(i);

            foreach (Quest newQuest in q.NextQuests)
                AddQuest(newQuest);

            if (q == _questActive)
            {
                q.IsQuestActive = false;
                _questActive = null;
            }
        }

        if (_questActive == null && _questsInProgress.Count > 0)
            ActivateQuest(_questsInProgress[0]);
    }

    private void QuestTaskProgress(Quest quest, AQuestTask questTask)
    {
        OnQuestTaskProgressed?.Invoke(quest, questTask);
    }

    private void QuestTaskCompleted(Quest quest, AQuestTask questTask)
    {
        OnQuestTaskCompleted?.Invoke(quest, questTask);

        StartCoroutine(questTask.AwardPlayer(this));

        if (quest != _questActive) return;

        SetQuestIndicator(quest);
    }

    private void SetQuestIndicator(Quest quest)
    {
        for (int i = 0; i < quest.QuestTasks.Count; ++i)
        {
            AQuestTask qt = quest.QuestTasks[i];
            qt.QuestTargetFinder?.Deactivate();

            if (!qt.IsCompleted)
            {
                if(qt.RefreshFinderPositionOnTaskUpdate)
                    qt.OnTaskProgress += RefreshMissionWaypoint;

                RefreshMissionWaypoint(qt);

                break;
            }
            else if (i == quest.QuestTasks.Count - 1)
            {
                RemoveWaypoint();
            }
        }
    }

    private void SetMissionWaypoint(AQuestTask qt)
    {
        if (qt.QuestTargetFinder == null)
            return;

        Transform transformToSelect = qt.QuestTargetFinder.FindObject(SceneServiceLocator.Instance);

        if (transformToSelect != null)
        {
            _missionWaypoint?.gameObject.SetActive(true);
            _transformToFollow = transformToSelect;
        }
        else
        {
            _missionWaypoint?.gameObject.SetActive(false);
            _transformToFollow = null;
        }
    }
    public void CompleteNextTask()
    {
        if (_questActive == null) return;
        _questActive.CompleteNextTask();
    }

    public void CompleteAllQuest()
    {
        StartCoroutine(CompleteAllQuest(_questActive));
    }

    public void CompleteActiveQuest() => StartCoroutine(CompleteQuest(_questActive));
    private IEnumerator CompleteQuest(Quest q)
    {
        OnQuestCompleted?.Invoke(q);
        _questsInProgress.Remove(q);
        _questsCompleted.Add(q);

        yield return StartCoroutine(q.AwardPlayer(this));

        foreach (Quest newQuest in q.NextQuests)
        {
            if (newQuest == null)
            {
                Debug.LogError($"Some quest is null in {q.name}");
                continue;
            }
            if (newQuest.ConditionalLocks.IsAnyPadlockLocked()) continue;
            AddQuest(newQuest);
        }

        if (q == _questActive)
        {
            q.IsQuestActive = false;
            _questActive = null;
        }

        if (_questActive == null && _questsInProgress.Count > 0)
            ActivateQuest(_questsInProgress[0]);
    }

    private IEnumerator CompleteAllQuest(Quest q)
    {
        OnQuestCompleted?.Invoke(q);
        _questsInProgress.Remove(q);
        _questsCompleted.Add(q);

        yield return StartCoroutine(q.AwardPlayer(this));

        foreach (Quest newQuest in q.NextQuests)
        {
            if (newQuest == null)
            {
                Debug.LogError($"Some quest is null in {q.name}");
                continue;
            }
            if (newQuest.ConditionalLocks.IsAnyPadlockLocked()) continue;
            AddQuest(newQuest);
        }

        if (q == _questActive)
        {
            q.IsQuestActive = false;
            _questActive = null;
        }

        if (_questActive == null && _questsInProgress.Count > 0)
        {
            ActivateQuest(_questsInProgress[0]);
            StartCoroutine(CompleteAllQuest(_questActive));
        }
    }

    //public void RefreshMissionWaypoint(AQuestTask qt) => RefreshMissionWaypoint(qt.QuestTargetFinder);
    public void RefreshMissionWaypoint(AQuestTask qt)
    {
        if (qt == null)
            return;

        StartCoroutine(RefreshWaypoint(qt));
    }

    private IEnumerator RefreshWaypoint(AQuestTask qt)
    {
        yield return new WaitForSeconds(.5f);

        RemoveWaypoint();

        if (qt.QuestTargetFinder == null)
        {
            RemoveWaypoint();
            yield break;
        }

        Transform transformToSelect = qt.QuestTargetFinder.FindObject(SceneServiceLocator.Instance, out ServiceLocator targetServiceLocator);
        qt.QuestTargetFinder.Activate();

        if (transformToSelect != null)
        {
            SetupWaypoint(transformToSelect, targetServiceLocator, qt.QuestTargetFinder.OffsetY);
        }
        else if(transformToSelect == null && !qt.QuestFinderCanBeNull)
        {
            StartCoroutine(qt.QuestTargetFinder.FinderNotFound(this));
        }
    }

    private void SetupWaypoint(Transform target, ServiceLocator targetServiceLocator, float offsetY)
    {
        _missionWaypoint = _windowManager.CreateWindow(_missionWaypointPrefab).GetComponent<QuestWaypoint>();
        _missionWaypoint.Target = target;
        _missionWaypoint.OffsetY = offsetY;
        _missionWaypoint.TargetServiceLocator = targetServiceLocator;

        if (_missionWaypoint.TargetServiceLocator.TryGetServiceLocatorComponent(out AdditionalOutline additionalOutline, allowToBeNull: true))
        {
            additionalOutline.SetOutline(color: Color.red);
        }
    }

    private void RemoveWaypoint()
    {
        if (_missionWaypoint == null) return;

        if (_missionWaypoint.TargetServiceLocator.TryGetServiceLocatorComponent(out AdditionalOutline additionalOutline, allowToBeNull: true))
        {
            additionalOutline.SetState(false);
        }
        _windowManager.DeleteWindow(_missionWaypoint);
    }

    public Quest GetActiveQuest() =>
        _questActive;
}
