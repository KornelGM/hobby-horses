using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePausingManager :  MonoBehaviour, IManager, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private TimeManager _timeManager;

    private bool _isPaused = false; 
    
    public void Pause()
    {
        if (_isPaused) return;
        
        _timeManager.PauseGame();
        _isPaused = true;
    }

    public void Unpause()
    {
        if (!_isPaused) return;
        _isPaused = false;
        _timeManager.UnPauseGame();
    }

    public void CustomReset()
    {
        
    }
}
