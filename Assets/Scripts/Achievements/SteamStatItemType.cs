using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Steam Stat Item Type", menuName = "ScriptableObjects/Achievement/Steam Stat Item Type")]
public class SteamStatItemType : Achievement
{
    [SerializeField] private List<ItemType> _itemTypes = new();
    public override bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if(!base.TryPerform(executedActionStat))
            return false;

        ItemDataActionStatData statDataItem = statData as ItemDataActionStatData;
        if (statDataItem == null) return false;

        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out ItemsManager itemsManager))
            return false;

        ItemData itemData = itemsManager.ItemDataDatabase.GetEntry(statDataItem.ItemDataGuid);

        if (itemData == null)
            return false;

        if (!_itemTypes.Contains(itemData.Type)) return false;

        _steamCommunicator.AddToStat(_steamName);
        return true;
    }
}
