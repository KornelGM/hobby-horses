using I2.Loc;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward Money", menuName = "ScriptableObjects/Quests/Rewards/Money")]
public class QuestRewardMoney : AQuestReward
{
    [SerializeField] PlayerDataManager _playerDataManager;
    [SerializeField] NotificationsManager _notificationsManager;
    [SerializeField] LocalizedString _notificationText;
    [SerializeField] int _rewardMoney;

    private readonly string AMOUNT = "{AMOUNT}";

    public override IEnumerator AwardPlayer()
    {
        _playerDataManager.Money += _rewardMoney;
        string notificationText = _notificationText.ToString().Replace(AMOUNT, _rewardMoney.ToString());
        _notificationsManager.AddNotification(notificationText, NotificationPosition.Middle, NotificationsType.Positive);

        yield break;
    }
}
