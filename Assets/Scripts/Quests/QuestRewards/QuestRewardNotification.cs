using I2.Loc;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Notification", menuName = "ScriptableObjects/Quests/Rewards/Notification")]
public class QuestRewardNotification : AQuestReward
{
    [SerializeField] private LocalizedString _notification;
    [SerializeField] private NotificationType _notificationType;
    [SerializeField] private float _delay;
    [SerializeField] private PadlockList _padlocks;
    [SerializeField] private PadlockList _reversedPadlocks;
    public override IEnumerator AwardPlayer()
    {
        if (_padlocks.IsAnyPadlockLocked()) yield break;
        if (_reversedPadlocks.Padlocks.Count != 0 && !_reversedPadlocks.IsAnyPadlockLocked()) yield break;
        if (SceneServiceLocator.Instance == null) yield break;
        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out NotificationsSystem notificationSystem)) yield break;

        notificationSystem.SendDelayedSideNotification(_notification, _notificationType, _delay);
    }
}
