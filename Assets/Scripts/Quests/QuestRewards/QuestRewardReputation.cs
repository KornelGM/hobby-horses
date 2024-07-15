using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Reputation", menuName = "ScriptableObjects/Quests/Rewards/Reputation")]
public class QuestRewardReputation : AQuestReward
{
    [SerializeField] int _rewardReputation;

    public override IEnumerator AwardPlayer()
    {
        if (!SceneServiceLocator.Instance) yield break;

        var SceneSL = SceneServiceLocator.Instance;
        if (!SceneSL.TryGetServiceLocatorComponent(out ReputationManager reputation)) yield break;

        reputation.ChangeReputation(_rewardReputation);

        yield break;
    }

    public void SetReward(int reputation)
    {
        _rewardReputation = reputation;
    }

}
