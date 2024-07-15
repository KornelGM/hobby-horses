using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestTask BuyItem", menuName = "ScriptableObjects/Quests/Tasks/Buy Item")]
public class QuestTaskBuyItem : AQuestTask
{
    [SerializeField, FoldoutGroup("Specific Settings")] private ItemData _itemData;
    [SerializeField, FoldoutGroup("Specific Settings")] private bool _showCounter;
    [SerializeField, FoldoutGroup("Specific Settings")] private int _amount = 1;
    bool _complete = false;

    private int _count = 0;

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
        //OrderItemsActionStatData orderData = data as OrderItemsActionStatData;

        //if (orderData == null)
        //    return false;

        //if (orderData.Value is not { Count: > 0})
        //    return false;

        //OrderItem orderItem = orderData.Value.FirstOrDefault(item => item.ItemDataGuid == _itemData.Guid);
        //if (orderItem == null) return false;

        //_count += orderItem.Amount;

        //_complete = _count >= _amount;
        //InvokeOnTaskProgress();

        return _complete;
    }

    public override void Initialize(ASaveQuestTask saveQuestTask)
    {
        if(saveQuestTask is SaveQuestTaskGroupValue save)
        {
            foreach (var item in save.Value)
            {
                if(item is SaveQuestTaskBoolValue boolValue)
                {
                    _complete = boolValue.Value;
                    continue;
                }

                if (item is SaveQuestTaskIntValue intValue)
                {
                    _count = intValue.Value;
                    continue;
                }
            }
        }

        IsCompleted = _complete;
    }

    public override string GetCounter()
    {
        return _showCounter ? $"({_count}/{_amount})" : "";
    }

    public override ASaveQuestTask GetQuestTaskSaveData()
    {
        List<ASaveQuestTask> save = new();

        SaveQuestTaskBoolValue boolSave = new(_complete);
        SaveQuestTaskIntValue intSave = new(_count);

        save.Add(boolSave);
        save.Add(intSave);

        return new SaveQuestTaskGroupValue(save);
    }
}
