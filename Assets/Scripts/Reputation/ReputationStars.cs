using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ReputationStars : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private ReputationManager _reputationManager;

    [SerializeField] private ReputationStar[] _stars;

    public void CustomAwake()
    {
        SubscribeEvents();
        RefreshFilling(_reputationManager.GetReputationLevel());
    }

    private void OnEnable()
    {
        RefreshFilling(_reputationManager.GetReputationLevel());
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _reputationManager.OnStarChanged += RefreshFilling;
    }

    private void UnsubscribeEvents()
    {
        _reputationManager.OnStarChanged -= RefreshFilling;
    }

    private void RefreshFilling(int star)
    {
        for (int i = 0; i <= _stars.Length - 1; i++)
        {
            float scaledValue = Mathf.InverseLerp(
                i == 0 ? 0 : _reputationManager.GetMaxReputationOnStar(i - 1), 
                _reputationManager.GetMaxReputationOnStar(i), _reputationManager.Reputation);

                _stars[i].RefreshFilling(scaledValue, _reputationManager.Reputation, _reputationManager.GetMaxReputationOnStar(i));
        }
    }
}
