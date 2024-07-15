using UnityEngine;

[CreateAssetMenu(fileName = "Steam Stat", menuName = "ScriptableObjects/Achievement/Steam Stat")]
public class SteamStat : Achievement
{
    public override bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if(!base.TryPerform(executedActionStat))
            return false;

        _steamCommunicator.AddToStat(_steamName);
        return true;
    }
}
