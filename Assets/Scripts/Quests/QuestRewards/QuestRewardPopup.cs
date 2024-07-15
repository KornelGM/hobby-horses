using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Popup", menuName = "ScriptableObjects/Quests/Rewards/Popup")]
public class QuestRewardPopup : AQuestReward
{
    [SerializeField] private PopUp _popup;
    [SerializeField] private PadlockList _padlocks;
    [SerializeField] private PadlockList _reversedPadlocks;
    public override IEnumerator AwardPlayer()
    {
        if (_padlocks.IsAnyPadlockLocked()) yield break;
        if (_reversedPadlocks.Padlocks.Count != 0 && !_reversedPadlocks.IsAnyPadlockLocked()) yield break;
        if (SceneServiceLocator.Instance == null) yield break;
        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out PopUpManager popupManager)) yield break;

        popupManager.AddPopUpToBlockingQueue(_popup, true);
    }
}
