using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsSaveService : SaveService<AchievementsSaveData, SaveData>
{
    protected override AchievementsSaveData SaveData(SaveData data)
    {
        if (data == null) return null;
        return data.AchievementsSaveData;
    }
}
