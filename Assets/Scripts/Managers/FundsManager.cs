using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FundsManager : MonoBehaviour, IServiceLocatorComponent, IStartable, ISaveable<SaveData>
{
    public event Action<int> OnCurrencyAmountChange;
    public ServiceLocator MyServiceLocator { get; set; }
    public int AvailableFunds => _availableFunds;
    public float FundsMultiplier { get; private set; }

    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;

    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private int _startingAmount;
    [SerializeField] private int _availableFunds;
    [SerializeField] private string _currency;

    public string Currency => _currency;

    public void CustomStart()
    {
        _currencyText.text = _currency;
        OnCurrencyAmountChange?.Invoke(_availableFunds);
    }

    public void SetFundsMultiplier(float fundsMultiplier)
    {
        FundsMultiplier = fundsMultiplier;
    }


    public bool CanBePurchased(float price)
    {
        if (_availableFunds - price >= 0)
        {
            return true;
        }

        _notificationsSystem.SendSideNotification("LackOfFunds", NotificationType.Warning);
        return false;       
    }

    public void AddAmount(int amount)
    {
        _availableFunds += Mathf.RoundToInt(amount * FundsMultiplier);
        OnCurrencyAmountChange?.Invoke(_availableFunds);
    }

    public void SubtractAmount(int amount, bool showNotification = false) 
    {
        _availableFunds -= amount;
        OnCurrencyAmountChange?.Invoke(_availableFunds);
        
        if (!showNotification) return;
    }

    #region Save&Load
    public SaveData CollectData(SaveData data)
    {
        data.Funds = _availableFunds;
        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null)
        {
            _availableFunds = _startingAmount;
            return;
        }

        _availableFunds = save.Funds;
    }
    #endregion
}
