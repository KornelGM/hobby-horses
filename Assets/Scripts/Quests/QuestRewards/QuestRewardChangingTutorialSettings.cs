using I2.Loc;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Tutorial", menuName = "ScriptableObjects/Quests/Rewards/Tutorial")]
public class QuestRewardChangingTutorialSettings : AQuestReward
{
    [SerializeField] private TutorialBoolType _boolType;
    [SerializeField] private bool _value;
    [SerializeField] private PadlockList _padlocks;
    [SerializeField] private PadlockList _reversedPadlocks;
    public override IEnumerator AwardPlayer()
    {
        if (_padlocks.IsAnyPadlockLocked()) yield break;
        if (_reversedPadlocks.Padlocks.Count != 0 && !_reversedPadlocks.IsAnyPadlockLocked()) yield break;
        if (SceneServiceLocator.Instance == null) yield break;
        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out TutorialManager _tutorialManager)) yield break;

        switch (_boolType)
        {
            case TutorialBoolType.Tutorial:
                _tutorialManager.EndTutorial(_value);
                break;
            case TutorialBoolType.ReputationBlocking:
                _tutorialManager.BlockReputation(_value);
                break;
            case TutorialBoolType.FirstChocolatNougat:
                _tutorialManager.FirstChocolateDone(_value);
                break;
            case TutorialBoolType.BlockGenerateOrders:
                _tutorialManager.GenerateOrdersBlock(_value);
                break;
        }
    }

    private enum TutorialBoolType
    {
        Tutorial,
        ReputationBlocking,
        FirstChocolatNougat,
        BlockGenerateOrders,
    }
}
