using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameStartupManager : MonoBehaviour, IServiceLocatorComponent, ISaveable<SaveData>
{

    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private FactoryInfo _factoryInfo;
    [ServiceLocatorComponent] private FundsManager _fundsManager;
    [ServiceLocatorComponent] private ReputationManager _reputationManager;
    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;

    [SerializeField] private NewGameStartup _startup;
    [SerializeField] private SpriteDatabase _spriteDatabase;

    private string _factoryName;
    private int _factoryIndex;
    private float _fundsMultiplier;
    private float _reputationMultiplier;

    public SaveData CollectData(SaveData data)
    {
        data.IsNewGame = false;
        data.FactoryName = _factoryName;
        data.FactoryIconIndex = _factoryIndex;
        data.FundsMultiplier = _fundsMultiplier;
        data.ReputationMultiplier = _reputationMultiplier;
        return data;
    }

    public void Initialize(SaveData save)
    {
        if(save == null)
        {
            _startup.SetDefaultVariables();
        }

        _factoryName = _startup.FactoryName;
        _factoryIndex = _startup.FactoryIconIndex;
        _fundsMultiplier = _startup.FundsMultiplier;
        _reputationMultiplier = _startup.ReputationMultiplier;

        if (save != null && !save.IsNewGame)
        {
            _factoryName = save.FactoryName;
            _factoryIndex = save.FactoryIconIndex;
            _fundsMultiplier = save.FundsMultiplier;
            _reputationMultiplier = save.ReputationMultiplier;
        }

        if (_reputationMultiplier > 1 || _reputationMultiplier < 0.1f)
        {
            _logger?.Log(LogType.Warning, "New Game Startup Manager", this, $"Reputation Multiplier out of range - set to 1");
            _reputationMultiplier = 1;
        }

        if (!_factoryInfo.TryChangeFactoryName(_factoryName))
            _factoryInfo.TryChangeFactoryName(_startup.GetDefaultFactoryName());

        _factoryInfo.SetNewIcon(_spriteDatabase.GetElementOfIndex(_factoryIndex));

        _fundsManager.SetFundsMultiplier(_fundsMultiplier);
        _reputationManager.SetReputationMultiplier(_reputationMultiplier);

        _logger?.Log(LogType.Log, "New Game Startup Manager", this, $"Game Setup: \nFactory Name {_factoryName} \nFactory Icon Index {_factoryIndex} \nFunds Multiplier {_fundsMultiplier} \nReputation Multiplier {_reputationMultiplier}");
    }

    private void OnApplicationQuit()
    {
        _startup.SetDefaultVariables();
    }
}
