using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats Database", menuName = "ScriptableObjects/Stats/Stats Database")]
public class StatsDatabase : DatabaseElement<ActionStat>
{
    public override List<string> GetPath() { return new();}
}
