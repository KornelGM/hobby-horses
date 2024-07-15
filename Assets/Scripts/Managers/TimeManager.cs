using System;
using UnityEngine;

public class TimeManager : MonoBehaviour, IAwake, IManager, IUpdateable, ITimeManager, IServiceLocatorComponent
{
    public bool Enabled { get => _enabled;}
    public ServiceLocator MyServiceLocator { get; set; }

    private const float PauseTimeScale = 0f;
    private const float NormalTimeScale = 1f;

    private float _actualTimeScale;
    private bool _enabled = true;

    public event Action<bool> OnGamePaused;

    public void CustomAwake()
    {
        UnPauseGame();
    }

    public void CustomReset() 
    {
    }
    
    public void CustomUpdate()
    {
        if (!_enabled) return;
    }

    public void PauseGame() 
    {
        ChangeTimeScale(PauseTimeScale);
        OnGamePaused?.Invoke(true);
    }

    public void UnPauseGame()
    {
        ChangeTimeScale(NormalTimeScale);
        OnGamePaused?.Invoke(false);
    }

    public float GetDeltaTime() => Time.deltaTime;

    public float GetUnscaledDeltaTime() => Time.unscaledDeltaTime;

    private void ChangeTimeScale(float newTimeScale)
    {
        _actualTimeScale = newTimeScale;
        Time.timeScale = _actualTimeScale;
    }
}