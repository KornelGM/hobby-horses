using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioSource : MonoBehaviour, IAwake, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;

    public void CustomAwake() => _audioSource = GetComponent<AudioSource>();
}
