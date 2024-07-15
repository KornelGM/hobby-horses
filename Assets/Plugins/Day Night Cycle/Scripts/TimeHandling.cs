using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandling : MonoBehaviour
{
    [SerializeField] TransformVariable dayNightCycleTransformVariable;
    [Range(0, 24)]
    public float timeOfDay;
    public float orbitSpeed = 1.0f;

    public const float HourMin = 0.0f;
    public const float HourMax = 24.0f;
        
    private bool isTimePaused;

    public float TimeOfDayDelta;
    private float oldTimeOfDay;

    // void Start()
    // {
    //     dayNightCycleTransformVariable.SetValue(this);

    //     oldTimeOfDay = timeOfDay;
    // }

    // void Update()
    // {
    //     oldTimeOfDay = timeOfDay;

    //     timeOfDay += Time.deltaTime * orbitSpeed;
    //     if (timeOfDay > HourMax)
    //     {
    //         timeOfDay -= HourMax;
    //         oldTimeOfDay -= HourMax;
    //     }
                     

    //     TimeOfDayDelta = timeOfDay - oldTimeOfDay;
    // }

    // public void SetTimePaused(bool isTimePaused)
    // {
    //     this.isTimePaused = isTimePaused;
    // }
}
