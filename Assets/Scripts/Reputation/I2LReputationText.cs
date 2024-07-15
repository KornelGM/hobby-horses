using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(LocalizationParamsManager))]
public class I2LReputationText : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private string _paramKey = "REPUTATION";

    private LocalizationParamsManager _paramsManager;
    [ServiceLocatorComponent] private ReputationManager _reputationManager;

    public void CustomStart()
    {
        if (_reputationManager == null) return;
        _paramsManager = GetComponent<LocalizationParamsManager>();
        _reputationManager.OnReputationChanged += RefreshText;
    }

    public void OnDestroy()
    {
        if (_reputationManager == null) return;
        _reputationManager.OnReputationChanged -= RefreshText;
    }

    private void RefreshText(int reputation)
    {
        _paramsManager.SetParameterValue(_paramKey, reputation.ToString());
    }
}
