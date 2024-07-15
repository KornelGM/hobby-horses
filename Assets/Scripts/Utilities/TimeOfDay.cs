using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "Time of Day", menuName = "ScriptableObjects/Time/Time Of Day")]
public class TimeOfDay : SerializedScriptableObject
{
    public string TimeOfDayName;
    
    [Header("Start time")]
    [Range(0, 23)] public readonly int StartingHour;
    [Range(0, 59)] public readonly int StartingMinutes;
    
    [Header("End time")]
    [Range(0, 23)] public readonly int EndingHour;
    [Range(0, 59)] public readonly int EndingMinutes;

    public float GetStartTime()
    {
        return StartingHour + StartingMinutes / 60;
    }

    public float GetEndTime()
    {
        return EndingHour + EndingMinutes / 60;
    }

    public override string ToString()
    {
        return TimeOfDayName;
    }
}
