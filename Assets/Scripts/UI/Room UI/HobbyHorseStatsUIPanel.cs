using System.Linq;
using UnityEngine;

public class HobbyHorseStatsUIPanel : MonoBehaviour
{
    [SerializeField] private HobbyHorseStatUI[] _stats;

    public void SetStats(HobbyHorseStats[] hobbyHorseStats)
    {
        if (hobbyHorseStats is not { Length: > 0 })
            return;

        float maxSpeed= 0;
        float maxRotateSpeed = 0;
        float brakeForce = 0;
        float accelerate = 0;
        float rotateAccelerate = 0;

        foreach (var stats in hobbyHorseStats)
        {
            maxSpeed += stats.MaxSpeed;
            maxRotateSpeed += stats.MaxRotateSpeed;
            brakeForce += stats.BrakeForce;
            accelerate += stats.Accelerate;
            rotateAccelerate += stats.RotateAccelerate;
        }

        GetStatUI(HobbyHorseStat.MaxSpeed).SetStatValue(maxSpeed);
        GetStatUI(HobbyHorseStat.MaxRotateSpeed).SetStatValue(maxRotateSpeed);
        GetStatUI(HobbyHorseStat.BrakeForce).SetStatValue(brakeForce);
        GetStatUI(HobbyHorseStat.Accelerate).SetStatValue(accelerate);
        GetStatUI(HobbyHorseStat.RotateAccelerate).SetStatValue(rotateAccelerate);
    }

    public void SetStats(HobbyHorseStats hobbyHorseStats)
    {
        if (hobbyHorseStats == null)
            return;

        GetStatUI(HobbyHorseStat.MaxSpeed).SetStatValue(hobbyHorseStats.MaxSpeed);
        GetStatUI(HobbyHorseStat.MaxRotateSpeed).SetStatValue(hobbyHorseStats.MaxRotateSpeed);
        GetStatUI(HobbyHorseStat.BrakeForce).SetStatValue(hobbyHorseStats.BrakeForce);
        GetStatUI(HobbyHorseStat.Accelerate).SetStatValue(hobbyHorseStats.Accelerate);
        GetStatUI(HobbyHorseStat.RotateAccelerate).SetStatValue(hobbyHorseStats.RotateAccelerate);
    }

    private HobbyHorseStatUI GetStatUI(HobbyHorseStat hobbyHorseStat)
    {
        return _stats.FirstOrDefault(stat => stat.HobbyHorseStat == hobbyHorseStat);
    }
}
