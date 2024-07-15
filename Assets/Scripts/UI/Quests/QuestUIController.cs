using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using I2.Loc;

public class QuestUIController : SerializedMonoBehaviour, IWindow, IServiceLocatorComponent
{
    private class QuestTaskToUI
    {
        public AQuestTask QuestTask;
        public QuestTaskUI QuestTaskUI;

        public QuestTaskToUI(AQuestTask questTask, QuestTaskUI questTaskUI)
        {
            QuestTask = questTask;
            QuestTaskUI = questTaskUI;
        }
    }

    [ServiceLocatorComponent] IQuestManager _questManager;
    [SerializeField] GameObject _questsUI;
    [SerializeField, AssetsOnly] QuestTaskUI _questTaskPrefab;
    [SerializeField] Localize _questTitle;
    [SerializeField] LayoutGroup _questTasksParent;
    [SerializeField] QuestCompleteTextController _questCompleteTextController;
    [SerializeField] float _hideUIWaitTime = 2f;
    [SerializeField] LayoutGroupFixer _layoutGroupFixer;

    private List<QuestTaskToUI> _questTaskToUis = new();
    private bool _questsUIActive = false;
    private WaitForSeconds _hideUIWait;

    public WindowManager Manager { get; set; }

    public GameObject MyObject => gameObject;

    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; } = true;
    public bool ShouldActivateCursor { get; set; } = false;
    public ServiceLocator MyServiceLocator { get; set; }

    private void Awake()
    {
        _questManager.IsNotNull(this, nameof(_questManager));
        _questsUI.IsNotNull(this, nameof(_questsUI));
        _questTaskPrefab.IsNotNull(this, nameof(_questTaskPrefab));
        _questTitle.IsNotNull(this, nameof(_questTitle));
        _questTasksParent.IsNotNull(this, nameof(_questTasksParent));
        _questCompleteTextController.IsNotNull(this, nameof(_questCompleteTextController));
        _layoutGroupFixer.IsNotNull(this, nameof(_layoutGroupFixer));

        _hideUIWait = new(_hideUIWaitTime);
        _questCompleteTextController.gameObject.SetActive(false);
        _questsUI.SetActive(false);

        _questManager.OnQuestActivated += ReloadActiveQuest;
        _questManager.OnQuestCompleted += QuestCompleted;
        _questManager.OnQuestTaskCompleted += QuestTaskCompleted;
        _questManager.OnQuestTaskProgressed += QuestTaskProgress;
    }

    private void Start()
    {        
        Quest activeQuest = _questManager.GetActiveQuest();
        if (activeQuest != null)
            ReloadActiveQuest(_questManager.GetActiveQuest());
    }

    private void ReloadActiveQuest(Quest quest)
    {
        _questsUI.SetActive(true);
        _questsUIActive = true;

        foreach (QuestTaskToUI questTaskToUI in _questTaskToUis)
            Destroy(questTaskToUI.QuestTaskUI.gameObject);

        _questTaskToUis.Clear();

        _questTitle.Term = quest.Title.mTerm;

        foreach (AQuestTask questTask in quest.QuestTasks)
        {
            CreateQuestTaskUI(questTask, 0);

            if (questTask is QuestTaskGroup)
            {
                QuestTaskGroup questGroup = questTask as QuestTaskGroup;

                foreach (AQuestTask qt in questGroup.Tasks)
                    CreateQuestTaskUI(qt, 1, questGroup.IsCompleted);
            }
        }

        StartCoroutine(_layoutGroupFixer.Fix());
    }

    private void CreateQuestTaskUI(AQuestTask questTask, int margin, bool isQuestCompleteOverride = false)
    {
        QuestTaskUI questTaskUI = Instantiate(_questTaskPrefab, _questTasksParent.transform);
        LocalizedString helpLocalization = questTask.GetQuestTaskHelp();
        questTaskUI.SetTaskName(questTask.GetQuestTaskDescription(), helpLocalization, !string.IsNullOrEmpty(helpLocalization.mTerm), questTask.GetCounter(), margin);

        if (isQuestCompleteOverride || questTask.IsCompleted)
            questTaskUI.FinishTask();

        _questTaskToUis.Add(new QuestTaskToUI(questTask, questTaskUI));
    }

    private void QuestCompleted(Quest quest)
    {
        if (quest != _questManager.GetActiveQuest())
            return;

        _questCompleteTextController.ShowText();
        _questsUIActive = false;

        StartCoroutine(HideQuestsUI());
    }

    private void QuestTaskCompleted(Quest quest, AQuestTask questTask)
    {
        QuestTaskToUI questTaskToUI = _questTaskToUis.Find(q => q.QuestTask == questTask);

        if (questTaskToUI == null)
            return;

        questTaskToUI.QuestTaskUI.FinishTask();

        if (questTaskToUI.QuestTask is not QuestTaskGroup)
            return;

        QuestTaskGroup questGroup = questTask as QuestTaskGroup;

        foreach (AQuestTask qt in questGroup.Tasks)
            QuestTaskCompleted(quest, qt);
    }

    private void QuestTaskProgress(Quest quest, AQuestTask questTask)
    {
        foreach (QuestTaskToUI questTaskToUI in _questTaskToUis)
        {
            if (questTask == questTaskToUI.QuestTask)
            {
                questTaskToUI.QuestTaskUI.UpdateCounter(questTask.GetCounter());
                break;
            }
        }
    }

    private IEnumerator HideQuestsUI()
    {
        yield return _hideUIWait;

        if (!_questsUIActive)
            _questsUI.SetActive(false);
    }

    public bool ShouldDeactivateCrosshair { get; set; }
}
