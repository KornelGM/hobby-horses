using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioPlayerPauseController : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private PlayerPausingManager _playerPausingManager;

    public ServiceLocator MyServiceLocator { get; set; }
    public UnityEvent OnPauseAudio;
    public UnityEvent OnPlayAudio;

    private void Start()
    {
        if (_playerPausingManager == null)
        {
            MyServiceLocator.TryGetServiceLocatorComponent(out _playerPausingManager);
        }

        if (_playerPausingManager == null)
            return;

        _playerPausingManager.OnPause += PauseAudio;
    }

    private void OnDestroy()
    {
        if (_playerPausingManager == null) return;

        _playerPausingManager.OnPause -= PauseAudio;
    }

    private void PauseAudio(bool value)
    {
        if (value)
        {
            OnPauseAudio?.Invoke();
        }
        else
        {
            OnPlayAudio?.Invoke();
        }
    }
}
