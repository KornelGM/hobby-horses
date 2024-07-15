using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Set Objects Active", menuName = "ScriptableObjects/Quests/Rewards/Set Objects Active")]
public class QuestRewardSetObjectsActive : AQuestReward
{
    [SerializeField] TransformVariable[] _objectsToActivate;
    [SerializeField] TransformVariable[] _objectsToDeactivate;

    public override IEnumerator AwardPlayer()
    {
        foreach (TransformVariable variable in _objectsToActivate)
            variable.Value.gameObject.SetActive(true);

        foreach (TransformVariable variable in _objectsToDeactivate)
            variable.Value.gameObject.SetActive(false);

        yield break;
    }
}