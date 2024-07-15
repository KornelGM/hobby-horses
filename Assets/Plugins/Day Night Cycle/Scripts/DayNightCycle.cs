using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;



public class DayNightCycle : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [Header("Sun")]
    public Transform sunRotationPivot;
    public Light sun;
    [SerializeField] private float sunRotationPivotOffset = 20.0f;

    [Header("Moon")]
    public Transform moonRotationPivot;
    public Light moon;
    [SerializeField] private float moonRotationPivotOffset = 20.0f;

    [Header("Stars")]
    public Volume skyVolume;
    private PhysicallyBasedSky sky;
    [SerializeField] private AnimationCurve starsCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0), new Keyframe(1, 1));
    
    [ServiceLocatorComponent] private WorldTimeManager _worldTimeManager;
    private const float StarsIntensity = 5000.0f;

    //constant to compensate for the difference in speed between directional lights and stars
    private const float StarsSpeedCompensation = 0.001f;

    private bool isNight = false;

    public bool Enabled { get; set; }

    void Awake()
    {
        _worldTimeManager.IsNotNull(this, nameof(_worldTimeManager));
    }

    void Start()
    {        
        skyVolume.profile.TryGet(out sky);
        sky.spaceRotation.overrideState = true;
    }

     void Update()
    {
        UpdateTime();        
    }

    private void OnValidate()
    {
        skyVolume.profile.TryGet(out sky);
        if (enabled) UpdateTime();
    }

    private void UpdateTime()
    {
        if(_worldTimeManager == null)
        return;

        float dayProgress = _worldTimeManager.Time / WorldTimeManager.HoursInDay;

        float sunRotation = Mathf.Lerp(-90, 270, dayProgress);
        float moonRotation = sunRotation + 180;
        
        //rotating sky and its elements along x axis with a separate RotationPivotOffset parameter to simulate 
        //various sun and moon slant angles
        sunRotationPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, sunRotationPivotOffset);
        sun.transform.localRotation = Quaternion.Euler(sunRotation, 0.0f, 0.0f);

        moonRotationPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, moonRotationPivotOffset);
        moon.transform.localRotation = Quaternion.Euler(moonRotation, 0.0f, 0.0f);


        sky.spaceEmissionMultiplier.value = starsCurve.Evaluate(dayProgress) * StarsIntensity;
        sky.spaceRotation.value = (moon.transform.rotation * Quaternion.Euler(moonRotation * StarsSpeedCompensation, moonRotationPivotOffset * StarsSpeedCompensation, 0.0f)).eulerAngles;
        
        CheckNightDayTransition();
    }

    private void CheckNightDayTransition()
    {
        if (isNight)
        {
            if (moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
            }
        }
        else
        {
            if (sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();
            }
        }
    }

    private void StartDay()
    {
        isNight = false;
        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }

    private void StartNight()
    {
        isNight = true;
        moon.shadows = LightShadows.Soft;
        sun.shadows = LightShadows.None;
    }      
}
