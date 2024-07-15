using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioStorage : MonoBehaviour, IServiceLocatorComponent
{
    [SerializeField] private AudioEvent _buttonHoverSoundEvent;
    [SerializeField] private AudioEvent _activeButtonClickSoundEvent;
    [SerializeField] private AudioEvent _blockedButtonClickSoundEvent;

    public ServiceLocator MyServiceLocator { get; set; }

    public void Start()
    {
        _buttonHoverSoundEvent.IsNotNull(this, nameof(_buttonHoverSoundEvent));
        _activeButtonClickSoundEvent.IsNotNull(this, nameof(_activeButtonClickSoundEvent));
        _blockedButtonClickSoundEvent.IsNotNull(this, nameof(_blockedButtonClickSoundEvent));
    }

    public AudioEventVariant GetRandomHoverAudioEventVariant() => _buttonHoverSoundEvent.EntryList[Random.Range(0, _buttonHoverSoundEvent.EntryList.Count)];
    public AudioEventVariant GetRandomActiveClickAudioEventVariant() => _activeButtonClickSoundEvent.EntryList[Random.Range(0, _activeButtonClickSoundEvent.EntryList.Count)];
    public AudioEventVariant GetRandomBlockedClickAudioEventVariant() => _blockedButtonClickSoundEvent.EntryList[Random.Range(0, _blockedButtonClickSoundEvent.EntryList.Count)];
}
