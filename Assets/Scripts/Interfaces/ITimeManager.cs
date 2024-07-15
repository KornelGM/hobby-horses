using System;

public interface ITimeManager
{
    public void PauseGame();
    public void UnPauseGame();
    public float GetDeltaTime();
    public float GetUnscaledDeltaTime();

    public event Action<bool> OnGamePaused;
}
