using System;

public interface IQuestManager
{
    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestActivated;
    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest, AQuestTask> OnQuestTaskCompleted;
    public event Action<Quest, AQuestTask> OnQuestTaskProgressed;

    public void AddQuest(Quest quest, bool setQuestActive = false);
    public void ActivateQuest(Quest quest);
    public Quest GetActiveQuest();
}
