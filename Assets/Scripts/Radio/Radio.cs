
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    private int _currentStationID = -1;

    [SerializeField] private GameObject _musicParticleEffects;
    private Animation _musicPlayingAnimation;

    public List<RadioStation> radioStations = new List<RadioStation>();

    void Awake() => _musicPlayingAnimation = GetComponent<Animation>();

    public void ChangeRadioStation() 
    { 
        _currentStationID = (_currentStationID + 1) % radioStations.Count;
        HandleStationIDChanged(_currentStationID);
    }

    private void MuteAllStations() => radioStations.ForEach(station => station.MuteRadioStation());

    private void HandleStationIDChanged(int value)
    {
        MuteAllStations();

        radioStations[value].UnMuteRadioStation();
        ToggleRadioEffect(value % 2 == 1);
    }

    private void ToggleRadioEffect(bool toggle)
    {
        _musicParticleEffects.SetActive(toggle);

        if(toggle) { _musicPlayingAnimation.Play(); }
        else { _musicPlayingAnimation.Stop(); }
    }
}
