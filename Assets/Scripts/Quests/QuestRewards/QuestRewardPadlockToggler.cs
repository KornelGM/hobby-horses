using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Padlock", menuName = "ScriptableObjects/Quests/Rewards/Padlock Toggler")]
public class QuestRewardPadlockToggler : AQuestReward
{
    [SerializeField] private Padlock[] _padlocksToLock;
    [SerializeField] private Padlock[] _padlocksToUnlock;

    public override IEnumerator AwardPlayer()
    {
        foreach (Padlock variable in _padlocksToLock)
            variable.Lock();

        foreach (Padlock variable in _padlocksToUnlock)
            variable.Unlock();

        yield break;
    }
}
