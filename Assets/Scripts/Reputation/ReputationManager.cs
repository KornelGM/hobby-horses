using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Reputation
{
    public int ReputationAmount;

    public Reputation(int amount = 0)
    {
        ReputationAmount = amount;
    }
}

public class ReputationManager : MonoBehaviour, IServiceLocatorComponent, ISaveable<SaveData>
{
    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;
    [ServiceLocatorComponent] private StatsManager _statsManager;
    [ServiceLocatorComponent] private LeaderboardManager _leaderboardManager;
    [ServiceLocatorComponent] private TutorialManager _tutorialManager;
    
    public ServiceLocator MyServiceLocator { get; set; }
    public float ReputationMultiplier { get; private set; } = 1;
    public int OrderRejectionPenalty => _orderRejectionPenalty;
    public int OrderCompletionReward => _orderCompletionReward;
    public int ReputationLevel => GetReputationLevel();
    public int ReputationLevelsCount => _starsTreshold.Length;
    public event Action<int> OnReputationChanged;
    public event Action<int> OnStarChanged;
    public event Action<int> OnLevelUp;

    [SerializeField] private int _startReputation = 30;
    [SerializeField, FoldoutGroup("Reputation Stat")] private ActionStat _starChanged;
    
    [SerializeField, FoldoutGroup("Reputation Settings")] private int _orderRejectionPenalty = -50;
    [SerializeField, FoldoutGroup("Reputation Settings")] private int _orderCompletionReward = 100;

    [SerializeField, FoldoutGroup("Pop Up")] private PopUpGameOver _gameOverPopUp;
    
    [SerializeField]
    private int[] _starsTreshold =
    {
        100,
        500,
        5000,
        20000,
        50000,
    };
    
    private PopUpManager _popUpManager;

    public int Reputation { get; private set; }
    public bool HasEnoughReputation(int reputation)
    {
        return Reputation > reputation;
    }

    private void Awake()
    {

    }

    public void SetReputationMultiplier(float reputationMultiplier)
    {
        ReputationMultiplier = reputationMultiplier;
    }

    public int GetReputationLevel()
    {
        for (int i = _starsTreshold.Length - 1; i >= 0; i--)
        {
            if (Reputation > _starsTreshold[i]) return i + 1;
        }
        return 0;
    }
    
    public int GetMaxStarValue()
    {
        if (_starsTreshold is not { Length: > 0 })
            return 0;

        return _starsTreshold.Length;
    }

    public int GetMaxReputationOnStar(int star)
    {
        if (_starsTreshold is not { Length: > 0 })
            return 0;

        return _starsTreshold[star];
    }

    [Button("Get Reputation")]
    public int ChangeReputation(int reputation, bool ignoreDifficultyMultiplier = false, bool showNotyfication = true)
    {
        if(_tutorialManager.ReputationBlocking)
        { return reputation; }

        int oldStar = GetReputationLevel();

        /*if (!ignoreDifficultyMultiplier && _difficultyManager != null)
        {
            reputation = (int)(reputation * _difficultyManager.GetDifficultySettingFloat(SettingName.Reputation));
        }*/

        Reputation += Mathf.RoundToInt(reputation * ReputationMultiplier);
        if (Reputation < 0) Reputation = 0;

        int newStar = GetReputationLevel();
        if (oldStar != newStar)
        {
            _statsManager.AddStat(_starChanged, new IntActionStatData(newStar));
            OnLevelUp?.Invoke(newStar);
        }

        OnStarChanged?.Invoke(newStar);
        OnReputationChanged?.Invoke(Reputation);

        if (Reputation <= 0) TryShowGameOverPopup();
        
        return reputation;
    }

    #region Save&Load
    public SaveData CollectData(SaveData data)
    {
        data.Reputation = new(Reputation);
        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null)
        {
            Reputation = _startReputation;
            return;
        }

        ChangeReputation(save.Reputation.ReputationAmount, showNotyfication: false);
    }
    #endregion

    private void TryShowGameOverPopup()
    {
        if (_gameOverPopUp == null) return;
        if (SceneServiceLocator.Instance == null) return;
        if (!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _popUpManager)) return;

        _popUpManager.AddPopUpToBlockingQueue(_gameOverPopUp);
    }
}
