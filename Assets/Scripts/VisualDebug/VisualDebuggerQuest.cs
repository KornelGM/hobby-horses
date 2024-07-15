using UnityEngine;

public class VisualDebuggerQuest : VisualDebugger, IStartable
{
    [ServiceLocatorComponent] private QuestManager _questManager;


    public override void CustomStart()
    {
        base.CustomStart();

        AddButton(this, b => CompleteAllQuest(), buttonName: "Complete All Quest", color: Color.red);
        AddButton(this, b => CompleteCurrentQuest(), buttonName: "Complete Current Quest", color: Color.yellow);
        AddButton(this, b => CompleteNextTask(), buttonName: "Complete next task", color: Color.green);
    }

    private void CompleteAllQuest()
    {
        _questManager.CompleteAllQuest();
    }

    private void CompleteCurrentQuest()
    {
        _questManager.CompleteActiveQuest();
    }

    private void CompleteNextTask()
    {
        _questManager.CompleteNextTask();
    }
}
