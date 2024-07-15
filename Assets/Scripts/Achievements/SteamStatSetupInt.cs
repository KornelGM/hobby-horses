using UnityEngine;

[CreateAssetMenu(fileName = "Steam Stat Setup Int", menuName = "ScriptableObjects/Achievement/Steam Stat Setup Int")]
public class SteamStatSetupInt : Achievement
{
    public override bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if(!base.TryPerform(executedActionStat))
            return false;

        IntActionStatData statDataInt = statData as IntActionStatData;
        if (statDataInt == null) return false;

        int currentValue =_steamCommunicator.GetIntStat(_steamName);
        if (currentValue > statDataInt.Value) return false;

        _steamCommunicator.SetStat(_steamName, statDataInt.Value);
        return true;
    }
}
