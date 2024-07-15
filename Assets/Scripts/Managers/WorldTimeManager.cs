using UnityEngine;
using System.Collections.Generic;

public class WorldTimeManager : MonoBehaviour, IManager, IStartable, IAwake, IServiceLocatorComponent, IUpdateable
{
    public const int HoursInDay = 24;
    public const int MinutesInHour = 60;

    public float TimeScale { get; set; } = 1f;
    public float Time { get; set; }
    public float DeltaTime { get; set; }
    public int Hour => (int)Time;
    public int Minutes => (int)((Time - Hour) * MinutesInHour);
    public int Days { get; set; } = 1;
    public TimeOfDay CurrentTimeOfDay {get; private set;}
    [field:SerializeField, Tooltip("In hours")] public float StartTime { get; private set; } = 12f;
    private float _oldTime = 0f;
    private float _accumulatedDeltaTime = 0f;
    [SerializeField]private List<TimeOfDay> _timesOfDay = new();


    [ServiceLocatorComponent] private TimeManager _timeManager;
    public ServiceLocator MyServiceLocator { get; set; }
    public bool Enabled => true;

    public void CustomAwake()
    {
        _timeManager.IsNotNull(this, nameof(_timeManager));
    }

    public void CustomStart()
    {
        ResetVariables();
        UpdateTime();
        SetTimeOfDay();
        _oldTime = Time;
    }

    public void CustomUpdate()
    {
        float deltaTime = _timeManager.GetDeltaTime() * TimeScale;
        _accumulatedDeltaTime += deltaTime;

        while (_accumulatedDeltaTime >= 1f)
        {
            UpdateTime();
            SetTimeOfDay();
            _accumulatedDeltaTime -= 1f;
        }
    }

    public void CustomReset()
    {
        ResetVariables();
    }

    public Timestamp GetTimestampOfCurrentTime()
    {
        return new Timestamp(Hour, Minutes, Days);
    }

    private bool CheckTimeOfDay(TimeOfDay timeOfDay)
    {
        float startingTime = timeOfDay.GetStartTime();
        float endingTime = timeOfDay.GetEndTime();

        if (endingTime < startingTime)//meaning it's something like 23 - 4 
        {
            return Time <= endingTime || Time >= startingTime;
        }

        return Time >= startingTime && Time <= endingTime;
    }

    private void ResetVariables()
    {
        TimeScale = 1f;
        Time = StartTime;
        _oldTime = 0f;
        Days = 1;
        _accumulatedDeltaTime = 0f;
    }

    private void UpdateTime()
    {
        _oldTime = Time;

        Time += 1f / 60f;
        if (Time >= HoursInDay)
        {
            Days++;
            _oldTime -= HoursInDay;
            Time -= HoursInDay;
        }

        DeltaTime = Time - _oldTime;
    }

    private void SetTimeOfDay()
    {
        foreach(var timeOfDay in _timesOfDay)
        {
            if(CheckTimeOfDay(timeOfDay))
            {
                CurrentTimeOfDay = timeOfDay;
                return;
            }
        }
        CurrentTimeOfDay = null;
        Debug.LogWarning("Something went wrong when trying to set time of day");
    }
}
