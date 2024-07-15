using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Steam Achievement", menuName = "ScriptableObjects/Achievement/Steam Achievement Check Item Data")]
public class SteamAchievementCheckItemData : Achievement
{
    [SerializeField, FoldoutGroup("Specific Settings")] private ItemData _itemData;

    public override bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if (!base.TryPerform(executedActionStat))
            return false;

        ItemDataActionStatData statDataMachine = statData as ItemDataActionStatData;
        if (statDataMachine == null) return false;


        if (statDataMachine.ItemDataGuid != _itemData.Guid) return false;

        _steamCommunicator.SetAchievement(_steamName);

        return true;
    }
}
