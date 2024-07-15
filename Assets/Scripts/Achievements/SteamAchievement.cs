using UnityEngine;

[CreateAssetMenu(fileName = "Steam Achievement", menuName = "ScriptableObjects/Achievement/Steam Achievement")]
public class SteamAchievement : Achievement
{
    public override bool TryPerform(ActionStat executedActionStat, AActionStatData statData = null)
    {
        if (!base.TryPerform(executedActionStat))
            return false;

        _steamCommunicator.SetAchievement(_steamName);

        return true;
    }
}
