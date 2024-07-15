using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private List<MusicZone> enteredZones = new();
    private MusicZone currentZone;

    private void Start()
    {
        foreach (MusicZone zone in FindObjectsOfType<MusicZone>())
        {
            zone.OnZoneEntered += OnZoneEntered;
            zone.OnZoneExit += OnZoneExit;
        }
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
        if (currentZone != null)
        {
            currentZone.PlayMusic(volume);
        }
    }

    private void OnZoneEntered(MusicZone zone, bool playingExisting)
    {
        foreach (MusicZone z in enteredZones)
        {
            if (z != zone)
            {
                z.ChangeVolume(zone.EnterSwitchTime, 0f);
            }
        }
        zone.PlayMusic(volume);
        currentZone = zone;
        if (!playingExisting)
        {
            enteredZones.Add(zone);
        }
    }

    private void OnZoneExit(MusicZone zone)
    {
        if (zone == currentZone && enteredZones.Count >= 2)
        {
            OnZoneEntered(enteredZones[^2], true);
        }
        else
        {
            zone.ChangeVolume(zone.EnterSwitchTime, 0f);
        }

        int index = enteredZones.LastIndexOf(zone);
        enteredZones.Remove(zone);
    }
}
