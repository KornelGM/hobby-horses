using UnityEngine;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour,IManager, ISettingsManager, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private Volume _globalVolume;
    [SerializeField] private VolumeProfile _skyVolume;
    [SerializeField] private VolumeProfile _defaultVolume;
    [SerializeField] private VolumeProfile _globalVolumeProfile;

    [SerializeField] private VolumeVariable _globalVolumeVariable;

    public VolumeProfile GetSkyVolume() => _skyVolume;
    public Volume GetGlobalVolume() =>
        _globalVolume;

    public VolumeProfile GetDefaultProfile() =>
        _defaultVolume;

    public void Reset() { }//
}
