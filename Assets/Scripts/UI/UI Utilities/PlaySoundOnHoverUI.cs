using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioPlayer))]
public class PlaySoundOnHoverUI : MonoBehaviour, IHoverUI, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private UIAudioSource _UIAudioSource;
    [ServiceLocatorComponent] private UIAudioStorage _UIAudioStorage;

    private AudioPlayer _audioPlayer;

    private void Start()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
        if (_audioPlayer == null) enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => PerformActionOnPointerEnter();

    public void OnPointerExit(PointerEventData eventData) => PerformActionOnPointerExit();

    public void PerformActionOnPointerEnter()
    {
        _audioPlayer.StopAll();
        if (_UIAudioStorage == null) return;

        _audioPlayer.PlayOneShot(_UIAudioStorage.GetRandomHoverAudioEventVariant(), _UIAudioSource.AudioSource);
    }

    public void PerformActionOnPointerExit()
    {
    }

    public ServiceLocator MyServiceLocator { get; set; }
}