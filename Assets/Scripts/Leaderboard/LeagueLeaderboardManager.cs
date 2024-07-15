using System.Collections.Generic;
using UnityEngine;
using System;
using I2.Loc;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class Factory
{
    public int IconIndex => _iconIndex;
    [field: SerializeField] public LocalizedString Name { get; private set; } = "Unnamed";
    [field: SerializeField, ReadOnly] public string MyFactoryName { get; private set; } = "My Factory";

    [field: SerializeField, ReadOnly] public int Place { get; set; } = 0;
    [field: SerializeField] public int Points { get; private set; } = 0;
    [field: SerializeField] public LeagueType LeagueType { get; set; }
    [field: SerializeField, ReadOnly] public Sprite Icon { get; private set; } = null;
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsMyFactory = false;
    public event Action OnMyFactoryNameChanged;
    public event Action OnMyFactoryPointsChanged;
    public event Action OnMyFactoryIconChanged;

    private int _iconIndex;

    public Factory(LocalizedString name, int points, SpriteDatabase spriteDatabase, int iconIndex ,Color color, LeagueType leagueType ,bool isMyFactory)
    {
        Name = name;
        Points = points;
        Icon = spriteDatabase.EntryList[iconIndex];
        Color = color;
        LeagueType = leagueType;
        IsMyFactory = isMyFactory;

        _iconIndex = iconIndex;
    }

    public Factory(FactorySaveData saveData, SpriteDatabase spriteDatabase, Color color)
    {
        Name = saveData.Name;
        Points = saveData.Points;
        Icon = spriteDatabase.EntryList[saveData.IconIndex];
        Color = color;
        LeagueType = saveData.LeagueType;
        IsMyFactory= saveData.IsMyFactory;

        _iconIndex = saveData.IconIndex;
    }

    public void Rename(string name)
    {
        MyFactoryName = name;
        OnMyFactoryNameChanged?.Invoke();
    }

    public void SetPoints(int points)
    {
        Points = points;
        OnMyFactoryPointsChanged?.Invoke();
    }

    public void ChangeIcon(Sprite old, Sprite newIcon) => SetIcon(newIcon);

    public void SetIcon(Sprite icon)
    {
        Icon = icon;
        OnMyFactoryIconChanged?.Invoke();
    }

    public Factory() { }

    public void SetAsMyFactory(ReputationManager reputation, FactoryInfo info)
    {
        if (reputation == null) return;
        if (info == null) return;
        IsMyFactory = true;
        reputation.OnReputationChanged += SetPoints;
        info.OnFactoryIconChanged += (prev, curr) => SetIcon(curr);
        info.OnFactoryNameSet += Rename;
    }
}

[System.Serializable]
public class FactorySaveData
{
    public string Name;
    public int Place;
    public int Points;
    public LeagueType LeagueType;
    public int IconIndex;
    public bool IsMyFactory = false;

    public FactorySaveData(Factory factory)
    {
        Name = factory.Name.mTerm;
        Place = factory.Place;
        Points = factory.Points;
        LeagueType = factory.LeagueType;
        IconIndex = factory.IconIndex;
        IsMyFactory = factory.IsMyFactory;
    }
}

public enum LeagueType
{
    Regional,
    National,
    World,
}

public class LeagueLeaderboardManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private LeaderboardManager _leaderboardManager;
    [ServiceLocatorComponent] private ReputationManager _reputationManager;
    public event Action OnEnemiesRefreshedPoints;

    [SerializeField] private LeagueType _leagueType;
    [SerializeField] private SpriteDatabase _spriteDatabase;
    [SerializeField] private int _factoriesAmount = 20;
    [field: SerializeField] public int MaxReputation { get; private set; } = 10000;
    [field: SerializeField] public int MinReputation { get; private set; } = 100;
    public List<Factory> Factories;

    public void CreateFactories()
    {
        for (int i = 0; i < _factoriesAmount; i++)
        {
            Factories.Add(new Factory(_leaderboardManager.RandomizeNewFactory(), 
                UnityEngine.Random.Range(MinReputation, MaxReputation), _spriteDatabase, 
                UnityEngine.Random.Range(1, _spriteDatabase.EntryList.Count), 
                Color.white, _leagueType, false));
        }
    }

    public void CreateFactories(List<FactorySaveData> saveData)
    {
        foreach (var factoryData in saveData)
        {
            Factory factory = new(factoryData, _spriteDatabase, Color.white);
            Factories.Add(factory);
        }
    }

    public void AddToLeague()
    {
        if (_leaderboardManager.MyFactory == null) return;
        if (Factories.Contains(_leaderboardManager.MyFactory)) return;
        Factories.Add(_leaderboardManager.MyFactory);
    }

    public void RemoveFromLeague()
    {
        if (_leaderboardManager.MyFactory == null) return;
        Factories.Remove(_leaderboardManager.MyFactory);
    }

    public bool IsMyFactoryInLeague()
    {
        foreach (var myFactory in Factories)
        {
            if (myFactory.IsMyFactory) return true;
        }
        return false;
    }

    public bool WouldOrIsMyFactoryInFirstPlace(int points)
    {
        var bestFactory = Factories.OrderByDescending(factory => factory.Points).FirstOrDefault();
        if (bestFactory == null) return false;
        if (bestFactory == _leaderboardManager.MyFactory) return true;
        if (bestFactory.Points < points) return true;
        return false;
    }

    private void ChangeRandomPoints()
    {
        foreach (var enemyFactory in Factories)
        {
            if (enemyFactory.IsMyFactory) continue;

            int pickedNumber = UnityEngine.Random.Range(0, 3);
            int points = enemyFactory.Points;
            int change = UnityEngine.Random.Range(100, 500);
            switch (pickedNumber)
            {
                case 1:
                    points += change;
                    break;
                case 2:
                    points -= change;
                    break;
            }

            enemyFactory.SetPoints(Mathf.Clamp(points, MinReputation, MaxReputation));
        }

        OnEnemiesRefreshedPoints?.Invoke();

    }

    public void UpdateFactoryPoints()
    {
        ChangeRandomPoints();
        _leaderboardManager.TryChangeLeague();
    }
}
