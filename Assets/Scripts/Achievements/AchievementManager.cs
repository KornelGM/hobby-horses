using UnityEngine;

public class AchievementManager : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private AchievementsDatabase _achievementsDatabase;
    [ServiceLocatorComponent] private StatsManager _statsManager;

    public void CustomAwake()
    {
        _statsManager.OnStatisticAdded += OnStatAdded;
    }

    private void OnDestroy()
    {
        _statsManager.OnStatisticAdded -= OnStatAdded;   
    }

    private void OnStatAdded(ActionStat actionStat)
    {
        foreach (Achievement achievement in _achievementsDatabase.EntryList)
        {
            if (achievement == null) return;
            achievement.TryPerform(actionStat, actionStat.Data);
        }
    }
}
