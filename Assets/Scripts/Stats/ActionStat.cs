using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stats/Stat")]
[Serializable]
public class ActionStat : DatabaseElement<GameObject>
{
    public AActionStatData Data;

    public override List<string> GetPath() => new() { Guid };
}

[Serializable]
public abstract class AActionStatData { }
