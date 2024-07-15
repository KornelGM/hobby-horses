using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour, IServiceLocatorComponent, IAwake, ISaveable<SaveData>
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private ReputationManager _reputationManager;
    [ServiceLocatorComponent] private FactoryInfo _factoryInfo;
    [ServiceLocatorComponent] private StatsManager _statsManager;

    public event Action<LeagueType> OnLeagueChanged;
    public event Action OnLeaderboardUpdate;

    public Factory MyFactory { get; set; } = new();

    [field: SerializeField] public LeagueLeaderboardManager RegionalLeagueLeaderboardManager { get; private set; }
    [field: SerializeField] public LeagueLeaderboardManager NationalLeagueLeaderboardManager { get; private set; }
    [field: SerializeField] public LeagueLeaderboardManager WorldLeagueLeaderboardManager { get; private set; }

    [SerializeField, FoldoutGroup("Settings")] private float _changeFactoryPointsTick = 60;
    [SerializeField, FoldoutGroup("Settings")] private Color _myFacrotyPanelColor;
    [SerializeField, FoldoutGroup("Settings")] private int _minNameID = 1;
    [SerializeField, FoldoutGroup("Settings")] private int _maxNameID = 30;
    [SerializeField, FoldoutGroup("Settings")] private string _factoryNamePrefix = "Factories Names";

    [SerializeField, FoldoutGroup("Stats")] private ActionStat _firstPlaceRegionalAchieved;
    [SerializeField, FoldoutGroup("Stats")] private ActionStat _firstPlaceNationalAchieved;
    [SerializeField, FoldoutGroup("Stats")] private ActionStat _firstPlaceWorldAchieved;

    [SerializeField, FoldoutGroup("Rewards")] private ItemData _regionalAward;
    [SerializeField, FoldoutGroup("Rewards")] private ItemData _nationalAward;
    [SerializeField, FoldoutGroup("Rewards")] private ItemData _worldAward;

    [SerializeField, ReadOnly] public bool ReceivedRegionalAward = false;
    [SerializeField, ReadOnly] public bool ReceivedNationalAward = false;
    [SerializeField, ReadOnly] public bool ReceivedWorldAward = false;

    private List<int> _leastIDs = new();

    public void CustomAwake()
    {
        MyFactory = CreateMyFactory(_reputationManager, _factoryInfo);
        RegionalLeagueLeaderboardManager.AddToLeague();

        _reputationManager.OnReputationChanged += RefreshFactoryPoints;
        _factoryInfo.OnFactoryNameSet += MyFactory.Rename;
        _factoryInfo.OnFactoryIconChanged += MyFactory.ChangeIcon;
        StartCoroutine(UpgradePointsLoop());
    }

    public void OnDestroy()
    {
        _factoryInfo.OnFactoryNameSet -= MyFactory.Rename;
        _factoryInfo.OnFactoryIconChanged -= MyFactory.ChangeIcon;
        _reputationManager.OnReputationChanged -= RefreshFactoryPoints;
    }

    public string RandomizeNewFactory()
    {
        if (_leastIDs == null) return "";
        if (_leastIDs.Count == 0) return "";

        int id = UnityEngine.Random.Range(0, _leastIDs.Count);
        string key = $"{_factoryNamePrefix}/{_leastIDs[id]}";

        _leastIDs.RemoveAt(id);
        return key;
    }

    private IEnumerator UpgradePointsLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_changeFactoryPointsTick);
            UpgradePoints();
        }
    }

    private Factory CreateMyFactory(ReputationManager reputationManager, FactoryInfo factoryInfo)
    {
        var myFactory = new Factory("My Manufacture", reputationManager.Reputation, factoryInfo.SpriteDatabase, 0, 
            _myFacrotyPanelColor, LeagueType.Regional ,true);

        myFactory.SetAsMyFactory(reputationManager, factoryInfo);
        return myFactory;
    }

    public void TryChangeLeague()
    {
        if (MyFactory == null) return;
        LeagueType leagueToBeSet = LeagueType.Regional;
        if (MyFactory.Points > RegionalLeagueLeaderboardManager.MinReputation)
        {
            TryGetRegionalAwards();
            leagueToBeSet = LeagueType.Regional;
        }

        if (MyFactory.Points > NationalLeagueLeaderboardManager.MinReputation)
        {
            TryGetNationalAwards();
            leagueToBeSet = LeagueType.National;
        }

        if (MyFactory.Points > WorldLeagueLeaderboardManager.MinReputation)
        {
            TryGetWorldAwards();
            leagueToBeSet = LeagueType.World;
        }

        SetMyFactoryLeague(leagueToBeSet);
    }

    private void SetMyFactoryLeague(LeagueType league)
    {
        if (GetLeagueManager(league).IsMyFactoryInLeague()) return;
        var regional = GetLeagueManager(LeagueType.Regional);
        var national = GetLeagueManager(LeagueType.National);
        var world = GetLeagueManager(LeagueType.World);

        switch (league)
        {
            case LeagueType.Regional:
                regional.AddToLeague();
                national.RemoveFromLeague();
                world.RemoveFromLeague();
                MyFactory.LeagueType = LeagueType.Regional;
                break;
            case LeagueType.National:
                regional.RemoveFromLeague();
                national.AddToLeague();
                world.RemoveFromLeague();
                MyFactory.LeagueType = LeagueType.National;
                break;
            case LeagueType.World:
                regional.RemoveFromLeague();
                national.RemoveFromLeague();
                world.AddToLeague();
                MyFactory.LeagueType = LeagueType.World;
                break;
        }
        OnLeagueChanged?.Invoke(league);
    }

    private void TryGetRegionalAwards()
    {
        if (ReceivedRegionalAward) return;
        if (!RegionalLeagueLeaderboardManager.WouldOrIsMyFactoryInFirstPlace(MyFactory.Points)) return;

        ReceivedRegionalAward = true;
        _statsManager.AddStat(_firstPlaceRegionalAchieved);
        Debug.Log("ReceivedRegionalAward : " + ReceivedRegionalAward);
    }

    private void TryGetNationalAwards()
    {
        if (ReceivedNationalAward) return;
        if (!NationalLeagueLeaderboardManager.WouldOrIsMyFactoryInFirstPlace(MyFactory.Points)) return;

        ReceivedNationalAward = true;
        _statsManager.AddStat(_firstPlaceNationalAchieved);
        Debug.Log("ReceivedNationalAward : " + ReceivedNationalAward);
    }

    private void TryGetWorldAwards()
    {
        if (ReceivedWorldAward) return;
        if (!WorldLeagueLeaderboardManager.WouldOrIsMyFactoryInFirstPlace(MyFactory.Points)) return;

        ReceivedWorldAward = true;
        _statsManager.AddStat(_firstPlaceWorldAchieved);
        Debug.Log("ReceivedWorldAward : " + ReceivedWorldAward);
    }

    private void UpgradePoints()
    {
        RegionalLeagueLeaderboardManager.UpdateFactoryPoints();
        NationalLeagueLeaderboardManager.UpdateFactoryPoints();
        WorldLeagueLeaderboardManager.UpdateFactoryPoints();

        OnLeaderboardUpdate?.Invoke();
    }

    private void RefreshFactoryPoints(int points)
    {
        MyFactory.SetPoints(points);

        OnLeaderboardUpdate?.Invoke();

        TryChangeLeague();
    }

    public LeagueLeaderboardManager GetLeagueManager(LeagueType type)
    {
        switch (type)
        {
            case LeagueType.Regional: return RegionalLeagueLeaderboardManager;
            case LeagueType.National: return NationalLeagueLeaderboardManager;
            case LeagueType.World: return WorldLeagueLeaderboardManager;
            default: return RegionalLeagueLeaderboardManager;
        }
    }

    public SaveData CollectData(SaveData data)
    {
        foreach (var factory in RegionalLeagueLeaderboardManager.Factories)
        {
            FactorySaveData saveData = new(factory);
            data.RegionalFactories.Add(saveData);
        }

        foreach (var factory in NationalLeagueLeaderboardManager.Factories)
        {
            FactorySaveData saveData = new(factory);
            data.NationalFactories.Add(saveData);
        }

        foreach (var factory in WorldLeagueLeaderboardManager.Factories)
        {
            FactorySaveData saveData = new(factory);
            data.WorldFactories.Add(saveData);
        }

        data.ReceivedRegionalAward = ReceivedRegionalAward;
        data.ReceivedNationalAward = ReceivedNationalAward;
        data.ReceivedWorldAward = ReceivedWorldAward;

        return data;
    }

    public void Initialize(SaveData save)
    {
        if(save == null)
        {
            _leastIDs.Clear();
            for (int i = _minNameID; i <= _maxNameID; i++) _leastIDs.Add(i);

            RegionalLeagueLeaderboardManager.CreateFactories();
            NationalLeagueLeaderboardManager.CreateFactories();
            WorldLeagueLeaderboardManager.CreateFactories();
            return;
        }

        if(save.RegionalFactories is {Count: > 0 })
        {
            RegionalLeagueLeaderboardManager.CreateFactories(save.RegionalFactories);
        }

        if (save.NationalFactories is { Count: > 0 })
        {
            NationalLeagueLeaderboardManager.CreateFactories(save.NationalFactories);
        }

        if (save.WorldFactories is { Count: > 0 })
        {
            WorldLeagueLeaderboardManager.CreateFactories(save.WorldFactories);
        }

        ReceivedRegionalAward = save.ReceivedRegionalAward;
        ReceivedNationalAward = save.ReceivedNationalAward;
        ReceivedWorldAward = save.ReceivedWorldAward;

        OnLeaderboardUpdate?.Invoke();
    }
}
