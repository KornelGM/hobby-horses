using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public enum CloudLayerWeather
{
    Clear,
    Streaky,
    StreakyDense,
    Sparse,
    CloudyLight,
    CloudyDense,
    Overcast,
    Rainy,
    Stormy
};

[System.Serializable]
public class WeatherParameters
{
    [Header("General Settings")]
    public CloudLayerWeather Weather;

    [Header("Cloud Layer")]
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 Streaky = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 StreakyDense = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 Sparce = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 CloudyLight = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 CloudyDense = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 Overcast = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 Rainy = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 Stormy = new Vector2(0.0f, 1.0f);

    [Header("Volumetric Clouds")]
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 VolumetricSparce = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 VolumetricCloudy = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 VolumetricOvercast = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 VolumetricOvercastThin = new Vector2(0.0f, 1.0f);
    [MinMaxSlider(0.0f, 1.0f, true)]
    public Vector2 VolumetricStormy = new Vector2(0.0f, 1.0f);

    [Header("Lighting Settings")]
    [MinMaxSlider(0.0f, 130000.0f, true)]
    public Vector2 SunIntensity = new Vector2(4000.0f, 13000.0f);
    public bool BoostMoonIntensity = false;
}

public class WeatherManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private WorldTimeManager _worldTimeManager;

    [Header("Rain")]
    [SerializeField] private PrecipitationManager precipitationManager;
    [SerializeField] private bool _rainExperimental = false;

    [Header("Sun and Moon")]
    public Light sun;
    public Light moon;
    [SerializeField] private AnimationCurve moonIntensityCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0), new Keyframe(1, 1));
    //[SerializeField] private float _moonIntensityChangeRate = 234000;
    private const float MinMoonIntensity = 13000;
    private const float MaxMoonIntensity = 130000;

    [Header("Sky Volume")]
    public Volume skyVolume;
    private Fog skyControll;

    [Header("Weather Simple Clouds")]
    [SerializeField] private bool _useVolumetricClouds = false;
    [SerializeField] private CloudLayerWeather currentWeather = CloudLayerWeather.Clear;
    [SerializeField] private WeatherParameters[] _weatherPresets;
    private CloudLayerWeather oldWeather;

    private float weatherChangeRate = 1.0f;
    [SerializeField] private float weatherChangeDuration;

    [Header("Cloud Layer")]
    public Volume cloudClearVolume;
    private float cloudClearWeight = 1.0f;
    public Volume cloudStreakyVolume;
    public Volume cloudStreakyDenseVolume;
    public Volume cloudSparceVolume;
    public Volume cloudCloudyLightVolume;
    public Volume cloudCloudyDenseVolume;
    public Volume cloudOvercastVolume;
    public Volume cloudRainyVolume;
    public Volume cloudStormyVolume;
    [SerializeField][Range(0, 1)] private float cloudStreakyWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudStreakyDenseWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudSparceWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudCloudyLightWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudCloudyDenseWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudOvercastWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudRainyWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudStormyWeight = 0.0f;

    [Header("Volumetric Clouds ")]
    public Volume cloudVolumetricClearVolume;
    private float cloudVolumetricClearWeight = 1.0f;
    public Volume cloudVolumetricSparseVolume;
    public Volume cloudVolumetricCloudyVolume;
    public Volume cloudVolumetricOvercastVolume;
    public Volume cloudVolumetricOvercastThinVolume;
    public Volume cloudVolumetricStormyVolume;
    [SerializeField][Range(0, 1)] private float cloudVolumetricSparceWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudVolumetricCloudyWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudVolumetricOvercastWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudVolumetricOvercastThinWeight = 0.0f;
    [SerializeField][Range(0, 1)] private float cloudVolumetricStormyWeight = 0.0f;

    [Header("Time to Change Weather")]
    [SerializeField][Range(0, 1000)] private float timeToChangeWeatherMin = 10.0f;
    [SerializeField][Range(0, 1000)] private float timeToChangeWeatherMax = 16.0f;

    WeatherParameters _oldWeatherSettings = new WeatherParameters();

    WeatherParameters _currentWeatherSettings = new WeatherParameters();

    [HideInInspector]public float weatherChangeProgress = 0;

    private float _weatherChangeDelay = 2.0f;
    public bool Enabled { get; set; }
    
    void Awake()
    {
        _worldTimeManager.IsNotNull(this, nameof(_worldTimeManager));
    }

    void Start()
    {        
        skyVolume.profile.TryGet(out skyControll);

        oldWeather = CloudLayerWeather.Clear;
        currentWeather = CloudLayerWeather.Clear;
        sun.intensity = 130000;
        _oldWeatherSettings = _weatherPresets.Where(w => w.Weather == currentWeather).FirstOrDefault();
        if (_oldWeatherSettings == null) _oldWeatherSettings = new();
        _currentWeatherSettings = _weatherPresets.Where(w => w.Weather == currentWeather).FirstOrDefault();
        if (_currentWeatherSettings == null) _currentWeatherSettings = new();       
                
    }

    void Update()
    {
        UpdateWeather();

        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            _useVolumetricClouds = !_useVolumetricClouds;
            SelectWeather();
        }*/
    }

    private void UpdateWeather()
    {
        if(_worldTimeManager == null)
        return;

        weatherChangeRate = 1.0f / weatherChangeDuration;

        //In this half-hour window we don't change weather, becauce simultaneous change in sun intensity in this window causes weird graphical glitch when changing from day to night
        if (_worldTimeManager.Time > 18.6f || _worldTimeManager.Time <= 17.9f)
        {
            weatherChangeProgress += weatherChangeRate * _worldTimeManager.DeltaTime;
            weatherChangeProgress = Mathf.Clamp01(weatherChangeProgress);
        }
        

        //manual clouds controls
        //if (updateClouds)
        {
            cloudClearVolume.weight = cloudClearWeight;
            cloudStreakyVolume.weight = cloudStreakyWeight;
            cloudStreakyDenseVolume.weight = cloudStreakyDenseWeight;
            cloudSparceVolume.weight = cloudSparceWeight;
            cloudCloudyLightVolume.weight = cloudCloudyLightWeight;
            cloudCloudyDenseVolume.weight = cloudCloudyDenseWeight;
            cloudOvercastVolume.weight = cloudOvercastWeight;
            cloudRainyVolume.weight = cloudRainyWeight;
            cloudStormyVolume.weight = cloudStormyWeight;

            cloudVolumetricClearVolume.weight = cloudVolumetricClearWeight;
            cloudVolumetricSparseVolume.weight = cloudVolumetricSparceWeight;
            cloudVolumetricCloudyVolume.weight = cloudVolumetricCloudyWeight;
            cloudVolumetricOvercastVolume.weight = cloudVolumetricOvercastWeight;
            cloudVolumetricOvercastThinVolume.weight = cloudVolumetricOvercastThinWeight;
            cloudVolumetricStormyVolume.weight = cloudVolumetricStormyWeight;
        }

        //Weather Controll
        if (oldWeather != currentWeather)
        {
            float weatherStrength = Random.value;
            Debug.Log($"Weather Strength: {weatherStrength * 100:0}%");
            _oldWeatherSettings = new WeatherParameters()
            {
                Weather = oldWeather,
                Streaky = new Vector2(cloudStreakyWeight, cloudStreakyWeight),
                StreakyDense = new Vector2(cloudStreakyDenseWeight, cloudStreakyDenseWeight),
                Sparce = new Vector2(cloudSparceWeight, cloudSparceWeight),
                CloudyLight = new Vector2(cloudCloudyLightWeight, cloudCloudyLightWeight),
                CloudyDense = new Vector2(cloudCloudyDenseWeight, cloudCloudyDenseWeight),
                Overcast = new Vector2(cloudOvercastWeight, cloudOvercastWeight),
                Rainy = new Vector2(cloudRainyWeight, cloudRainyWeight),
                Stormy = new Vector2(cloudStormyWeight, cloudStormyWeight),

                VolumetricSparce = new Vector2(cloudVolumetricSparceWeight, cloudVolumetricSparceWeight),
                VolumetricCloudy = new Vector2(cloudVolumetricCloudyWeight, cloudVolumetricCloudyWeight),
                VolumetricOvercast = new Vector2(cloudVolumetricOvercastWeight, cloudVolumetricOvercastWeight),
                VolumetricOvercastThin = new Vector2(cloudVolumetricOvercastThinWeight, cloudVolumetricOvercastThinWeight),
                VolumetricStormy = new Vector2(cloudVolumetricStormyWeight, cloudVolumetricStormyWeight),

                //SunIntensity = new Vector2(sun.intensity, sun.intensity),
                //BoostMoonIntensity = _currentWeatherSettings.BoostMoonIntensity,
            };

            _currentWeatherSettings = _weatherPresets.Where(w => w.Weather == currentWeather).FirstOrDefault();
            _currentWeatherSettings = new WeatherParameters()
            {
                Weather = _currentWeatherSettings.Weather,
                Streaky = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.Streaky.x, _currentWeatherSettings.Streaky.y),
                StreakyDense = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.StreakyDense.x, _currentWeatherSettings.StreakyDense.y),
                Sparce = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.Sparce.x, _currentWeatherSettings.Sparce.y),
                CloudyLight = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.CloudyLight.x, _currentWeatherSettings.CloudyLight.y),
                CloudyDense = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.CloudyDense.x, _currentWeatherSettings.CloudyDense.y),
                Overcast = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.Overcast.x, _currentWeatherSettings.Overcast.y),
                Rainy = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.Rainy.x, _currentWeatherSettings.Rainy.y),
                Stormy = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.Stormy.x, _currentWeatherSettings.Stormy.y),

                VolumetricSparce = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.VolumetricSparce.x, _currentWeatherSettings.VolumetricSparce.y),
                VolumetricCloudy = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.VolumetricCloudy.x, _currentWeatherSettings.VolumetricCloudy.y),
                VolumetricOvercast = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.VolumetricOvercast.x, _currentWeatherSettings.VolumetricOvercast.y),
                VolumetricOvercastThin = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.VolumetricOvercastThin.x, _currentWeatherSettings.VolumetricOvercastThin.y),
                VolumetricStormy = CalculateParameterStrength(weatherStrength, _currentWeatherSettings.VolumetricStormy.x, _currentWeatherSettings.VolumetricStormy.y),

               // SunIntensity = new Vector2(_currentWeatherSettings.SunIntensity.y, _currentWeatherSettings.SunIntensity.x),
               // BoostMoonIntensity = _currentWeatherSettings.BoostMoonIntensity,
            };
            oldWeather = currentWeather;
            weatherChangeProgress = 0.0f;
        }

        if (_currentWeatherSettings != null && weatherChangeProgress <= 1)
        {
            if (!_useVolumetricClouds)
            {
                cloudStreakyWeight = Mathf.Lerp(_oldWeatherSettings.Streaky.y, _currentWeatherSettings.Streaky.y, weatherChangeProgress);
                cloudStreakyDenseWeight = Mathf.Lerp(_oldWeatherSettings.StreakyDense.y, _currentWeatherSettings.StreakyDense.y, weatherChangeProgress);
                cloudSparceWeight = Mathf.Lerp(_oldWeatherSettings.Sparce.y, _currentWeatherSettings.Sparce.y, weatherChangeProgress);
                cloudCloudyLightWeight = Mathf.Lerp(_oldWeatherSettings.CloudyLight.y, _currentWeatherSettings.CloudyLight.y, weatherChangeProgress);
                cloudCloudyDenseWeight = Mathf.Lerp(_oldWeatherSettings.CloudyDense.y, _currentWeatherSettings.CloudyDense.y, weatherChangeProgress);
                cloudOvercastWeight = Mathf.Lerp(_oldWeatherSettings.Overcast.y, _currentWeatherSettings.Overcast.y, weatherChangeProgress);
                cloudRainyWeight = Mathf.Lerp(_oldWeatherSettings.Rainy.y, _currentWeatherSettings.Rainy.y, weatherChangeProgress);
                cloudStormyWeight = Mathf.Lerp(_oldWeatherSettings.Stormy.y, _currentWeatherSettings.Stormy.y, weatherChangeProgress);

               // sun.intensity = Mathf.Lerp(_oldWeatherSettings.SunIntensity.y, _currentWeatherSettings.SunIntensity.y, weatherChangeProgress);

                cloudVolumetricSparceWeight = 0;
                cloudVolumetricCloudyWeight = 0;
                cloudVolumetricOvercastWeight = 0;
                cloudVolumetricOvercastThinWeight = 0;
                cloudVolumetricStormyWeight = 0;

                sun.GetComponent<HDAdditionalLightData>().affectsVolumetric = false;
                moon.GetComponent<HDAdditionalLightData>().affectsVolumetric = false;

                skyControll.enableVolumetricFog.overrideState = false;
                skyControll.enableVolumetricFog.value = false;
                
                if (_rainExperimental)
                {
                    if (currentWeather == CloudLayerWeather.Rainy)
                    {
                        precipitationManager.rain.amount = Mathf.Lerp(0, 0.1f, weatherChangeProgress);
                    }
                    else if (currentWeather != CloudLayerWeather.Rainy && precipitationManager.rain.amount != 0)
                    {
                        precipitationManager.rain.amount = Mathf.Lerp(0.1f, 0.0f, weatherChangeProgress);
                    }

                }
            }
            else
            {
                cloudStreakyWeight = 0;
                cloudStreakyDenseWeight = 0;
                cloudSparceWeight = 0;
                cloudCloudyLightWeight = 0;
                cloudCloudyDenseWeight = 0;
                cloudOvercastWeight = 0;
                cloudRainyWeight = 0;
                cloudStormyWeight = 0;

                //reseting sun intensity 
                //sun.intensity = 130000;

                cloudVolumetricSparceWeight = Mathf.Lerp(_oldWeatherSettings.VolumetricSparce.y, _currentWeatherSettings.VolumetricSparce.y, weatherChangeProgress);
                cloudVolumetricCloudyWeight = Mathf.Lerp(_oldWeatherSettings.VolumetricCloudy.y, _currentWeatherSettings.VolumetricCloudy.y, weatherChangeProgress);
                cloudVolumetricOvercastWeight = Mathf.Lerp(_oldWeatherSettings.VolumetricOvercast.y, _currentWeatherSettings.VolumetricOvercast.y, weatherChangeProgress);
                cloudVolumetricOvercastThinWeight = Mathf.Lerp(_oldWeatherSettings.VolumetricOvercastThin.y, _currentWeatherSettings.VolumetricOvercastThin.y, weatherChangeProgress);
                cloudVolumetricStormyWeight = Mathf.Lerp(_oldWeatherSettings.VolumetricStormy.y, _currentWeatherSettings.VolumetricStormy.y, weatherChangeProgress);

                sun.GetComponent<HDAdditionalLightData>().affectsVolumetric = true;
                moon.GetComponent<HDAdditionalLightData>().affectsVolumetric = true;

                skyControll.enableVolumetricFog.overrideState = true;
                skyControll.enableVolumetricFog.value = true;
            }
        }

       /* if (timeHandling.timeOfDay >= 6.0f && timeHandling.timeOfDay <= 18.0f)
        {
            moon.intensity = sun.intensity;
        }
        else if (timeHandling.timeOfDay > 18.0f)
        {
            moon.intensity -= _moonIntensityChangeRate * timeHandling.TimeOfDayDelta;
            if (moon.intensity < MinMoonIntensity) moon.intensity = MinMoonIntensity;
        }
        else if (timeHandling.timeOfDay > 5.5f)
        {
            moon.intensity += _moonIntensityChangeRate * timeHandling.TimeOfDayDelta;
            if (moon.intensity > MaxMoonIntensity) moon.intensity = MaxMoonIntensity;
        }
        else
        {
            moon.intensity = MinMoonIntensity;

        }*/

        //Debug.Log($"moon {moon.intensity}, sun {sun.intensity}, hour {timeHandling.timeOfDay}");

        _weatherChangeDelay -= _worldTimeManager.DeltaTime;
        if (_weatherChangeDelay <= 0)
        {
            SelectWeather();
            _weatherChangeDelay = Random.Range(timeToChangeWeatherMin, timeToChangeWeatherMax);
        }

    }
    private Vector2 CalculateParameterStrength(float weatherStrength, float min, float max)
    {
        return new Vector2(1, 1) * (max + (weatherStrength * (max - min)) - (max - min));
    }


    [Button("Roll Weather")]
    private void SelectWeather()
    {
        CloudLayerWeather newWeather;     

        do
        {
            newWeather = (CloudLayerWeather)Random.Range(0, 8);
        } while (newWeather == currentWeather);
        currentWeather = newWeather;
    }      

}
