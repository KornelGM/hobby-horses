using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioStation : MonoBehaviour
{
    AudioSource _audioSource;

    void Awake() => _audioSource = GetComponent<AudioSource>();

    public void MuteRadioStation() => _audioSource.mute = true;
    public void UnMuteRadioStation() => _audioSource.mute = false;
}
