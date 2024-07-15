using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicZoneMuffler : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private AudioMixerManager _audioMixerManager;

    [SerializeField] private Tags _tags;
    [SerializeField] private bool _shouldMuffle;
}
