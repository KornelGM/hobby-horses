using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestTargetFinder PickupType", menuName = "ScriptableObjects/Quests/Target Finders/Pickup Type")]
public class QuestTargetFinderPickupType : AQuestTargetFinder
{
    [SerializeField] ItemsManager _itemsGenerator;
    [SerializeField] ServiceLocatorVariable _playerServiceLocator;
    [SerializeField] ItemType _itemType;

    public override Transform FindObject(ServiceLocator sceneServiceLocator)
    {
        ItemServiceLocator closestItem = _itemsGenerator.Items
            .Where(c => c.isActiveAndEnabled && c.TryGetComponent(out ItemDataContainer container) && container.ItemData != null && container.ItemData.Type == _itemType)
            .OrderBy(GetSqrMagnitude)
            .FirstOrDefault();

        if (closestItem != null)
            return closestItem.transform;

        return null;
    }

    private float GetSqrMagnitude(ItemServiceLocator container)
    {
        return (container.transform.position - _playerServiceLocator.Value.transform.position).sqrMagnitude;
    }
}
