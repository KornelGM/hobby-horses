using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class StatsManager : MonoBehaviour, IServiceLocatorComponent, IManager, IStatsManager, ISaveable<SaveData>
{
    public ServiceLocator MyServiceLocator { get; set; }
    public event Action<ActionStat> OnStatisticAdded;

    public bool DebugEnabled = false;
    public bool Enabled => true;

#if UNITY_EDITOR
    [field: OdinSerialize, ReadOnly, HideInEditorMode]
#endif
    public Dictionary<string, List<AActionStatData>> Statistics { get; set; } = new();

    public void CustomReset()
    {
        Statistics = new();
    }

    public void AddStat(ActionStat stat, AActionStatData statData = null)
    {
        if(stat == null)
        {
            Debug.LogError("Stat you want to add is null");
            return;
        }

        ActionStat statInstance = Instantiate(stat);
        statInstance.Data = statData;

        if (DebugEnabled)
            Debug.Log(statInstance);

        OnStatisticAdded?.Invoke(statInstance);

        Statistics.TryAdd(statInstance.Guid, new());
        Statistics[statInstance.Guid].Add(statData);
    }

    public SaveData CollectData(SaveData data)
    {
        data.PlayerSaveData.Statistics = Statistics;
        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null || save.PlayerSaveData == null || save.PlayerSaveData.Statistics == null)
            return;

        if(!save.IsNewGame)
            Statistics = save.PlayerSaveData.Statistics;
    }

    public List<string> GetPath()
    {
        Debug.Log("Log path not available in this script");
        return new();
    }
}
