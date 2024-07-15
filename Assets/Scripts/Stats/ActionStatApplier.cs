using UnityEngine;

public class ActionStatApplier : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private StatsManager _statsManager;
    [SerializeField] private ActionStat _statToApply;

    public ServiceLocator MyServiceLocator { get; set; }

    public void ApplyStats()
    {
        if (MyServiceLocator == null)
        {
            MyServiceLocator = SceneServiceLocator.Instance;
            
            if (MyServiceLocator == null)
            {
                Debug.LogError("SceneServiceLocator not found for ActionStatApplier");
                return;
            }

            if (!MyServiceLocator.TryGetServiceLocatorComponent(out _statsManager))
            {
                Debug.LogError("StatsManager not found in SceneServiceLocator for ActionStatApplier");
                return;
            }
        }
        
        _statsManager.AddStat(_statToApply);
    }
}
