using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement DB", menuName = "ScriptableObjects/Achievement/Achievement DB")]
public class AchievementsDatabase : DatabaseElement<Achievement>
{
    public override List<string> GetPath()
    {
        return new List<string>();
    }
}
