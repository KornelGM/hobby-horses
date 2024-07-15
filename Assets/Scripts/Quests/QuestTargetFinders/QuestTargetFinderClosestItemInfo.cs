using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Closest ItemInfo", menuName = "ScriptableObjects/Quests/Target Finders/Closest")]
public class QuestTargetFinderClosestItemInfo : AQuestTargetFinder
{
    [SerializeField] private ItemData _itemToSearch;
    [SerializeField] private ItemType _itemTypeToSearch;

    private PlayerManager _playerManager;
    private QuestManager _questManager;

    public override Transform FindObject(ServiceLocator sceneServiceLocator, out ServiceLocator targetServiceLocator)
    {
        targetServiceLocator = null;
        if (!sceneServiceLocator.TryGetServiceLocatorComponent(out ItemsManager itemsManager)) return null;
        if (!sceneServiceLocator.TryGetServiceLocatorComponent(out PlayerManager playerManager)) return null;

        if (playerManager.LocalPlayer == null)
        {
            if (!sceneServiceLocator.TryGetServiceLocatorComponent(out QuestManager questManager)) return null;
            _questManager = questManager;
            _playerManager = playerManager;
            return null;
        }

        List<ItemServiceLocator> items = new();
        foreach (ItemServiceLocator item in itemsManager.Items)
        {
            if (item == null) continue;
            if (item.gameObject == null) continue;
            if (!item.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer, true, false)) continue;
            if (item.TryGetServiceLocatorComponent(out ItemDetectionInfo itemDetectionInfo, true, false))
            {
                if (!itemDetectionInfo.AbleToDetect || !itemDetectionInfo.gameObject.activeSelf) 
                    continue;
            }
            if (itemDataContainer.ItemData != _itemToSearch && _itemToSearch != null) continue;
            if (itemDataContainer.ItemData.Type != _itemTypeToSearch && _itemTypeToSearch != ItemType.None) continue;

            items.Add(item);
        }
        var sorted = items.OrderBy(item => SqrDistToPlayer(item.gameObject.transform));
        if (sorted.Count() == 0) return null;

        targetServiceLocator = sorted.First();
        return sorted.First().transform;

        float SqrDistToPlayer(Transform item)
        {
             return Vector3.SqrMagnitude(playerManager.LocalPlayer.transform.position - item.position);
        }
    }
}
