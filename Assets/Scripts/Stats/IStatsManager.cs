using System;
using System.Collections.Generic;

public interface IStatsManager
{
    public event Action<ActionStat> OnStatisticAdded;
    public Dictionary<string, List<AActionStatData>> Statistics { get; set; }
    public void AddStat(ActionStat stat, AActionStatData statData = null);
}
