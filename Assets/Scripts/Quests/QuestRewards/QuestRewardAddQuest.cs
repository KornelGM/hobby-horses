using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Add Quest", menuName = "ScriptableObjects/Quests/Rewards/Add Quest")]
public class QuestRewardAddQuest : AQuestReward
{
    [SerializeField] private Quest _quest;
    [SerializeField] private PadlockList _padlocks;
    [SerializeField] private PadlockList _reversedPadlocks;
    public override IEnumerator AwardPlayer()
    {
        if (_padlocks.IsAnyPadlockLocked()) yield break;
        if (_reversedPadlocks.Padlocks.Count != 0 && !_reversedPadlocks.IsAnyPadlockLocked()) yield break;
        if (SceneServiceLocator.Instance == null) yield break;
        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out QuestManager questManager)) yield break;

        questManager.AddQuest(_quest, true);
    }
}
