using System.Collections;
using UnityEngine;

public class GameplayTimeAchievement : MonoBehaviour, IServiceLocatorComponent, ISaveable<AchievementsSaveData>, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private float _secondsBetweenCalculate = 60;

    private WaitForSeconds _waitForSeconds;
    private float _gamePlayMinutes = 0;

    private IEnumerator CalculateMinutesOfGameplay()
    {
        while (true) 
        {
            yield return _waitForSeconds;

            _gamePlayMinutes += _secondsBetweenCalculate / 60;
        }
    }

    public AchievementsSaveData CollectData(AchievementsSaveData data)
    {
        data.GameplayTime = _gamePlayMinutes;
        return data;
    }

    public void Initialize(AchievementsSaveData save)
    {
        if (save == null)
            return;

        _gamePlayMinutes = save.GameplayTime;
    }

    public void CustomStart()
    {
        _waitForSeconds = new WaitForSeconds(_secondsBetweenCalculate);

        StartCoroutine(CalculateMinutesOfGameplay());
    }

    public float GetGameplayTime() => _gamePlayMinutes;
}
