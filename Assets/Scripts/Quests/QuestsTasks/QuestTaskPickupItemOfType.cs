using UnityEngine;
using I2.Loc;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestTask PickupItemOfType", menuName = "ScriptableObjects/Quests/Tasks/Pickup Item Of Type")]
public class QuestTaskPickupItemOfType : AQuestTask
{
    [SerializeField] ItemType _itemType;
    [SerializeField] int _pickupAmount;
    [SerializeField] bool _showCounter;

    private int _pickedUpCount = 0;
    private bool _complete;
    private List<string> _itemsGuid = new();

    public override void ForceCompleteTask()
    {
        _pickedUpCount = _pickupAmount;
        InvokeOnTaskProgress();
        base.ForceCompleteTask();
    }

    public override bool TryCompleteTask(ActionStat actionStat = null)
    {
        if (actionStat == null) return false;

        if (actionStat.Guid == _usedActionStat.Guid)
        {
            if (!CompareActionStatData(actionStat.Data))
                return false;

            if (_complete)
            {
                ClearOnTaskProgress();
                IsCompleted = true;
            }
        }
        return IsCompleted;
    }

    public override bool CompareActionStatData(AActionStatData data = null)
    {
        ItemDataAndGuidActionStatData pickupData = data as ItemDataAndGuidActionStatData;

        if (string.IsNullOrEmpty(pickupData.ItemDataGuid))
            return false;

        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out ItemsManager itemsManager)) 
            return false;

        ItemData itemData = itemsManager.ItemDataDatabase.GetEntry(pickupData.ItemDataGuid);

        if (itemData.Type != _itemType)
            return false;

        if (_itemsGuid.Contains(pickupData.Guid))
            return false;

        _pickedUpCount += itemData.Amount;

        _itemsGuid.Add(pickupData.Guid);

        _complete = _pickedUpCount >= _pickupAmount;
        InvokeOnTaskProgress();

        return _complete;
    }

    public override string GetCounter()
    {
        return _showCounter ? $"({_pickedUpCount}/{_pickupAmount})" : "";
    }

    public override void Initialize(ASaveQuestTask saveQuestTask)
    {
        if(saveQuestTask is SaveQuestTaskGroupValue save)
        {
            foreach (var data in save.Value)
            {
                if (data is SaveQuestTaskIntValue intData)
                {
                    _pickedUpCount = intData.Value;
                    continue;
                }

                if (data is SaveQuestTaskStringList stringListData)
                {
                    _itemsGuid = new(stringListData.Value);
                    continue;
                }
            }
        }

        _complete = _pickedUpCount >= _pickupAmount;

        IsCompleted = _complete;
    }

    public override ASaveQuestTask GetQuestTaskSaveData()
    {
        List<ASaveQuestTask> save = new();

        SaveQuestTaskIntValue countSave = new (_pickedUpCount);
        SaveQuestTaskStringList items = new(_itemsGuid);

        save.Add(countSave);
        save.Add(items);

        return new SaveQuestTaskGroupValue(save);
    }
}
