using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFactorySummary : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private LeaderboardManager _leaderboardManager;
    [ServiceLocatorComponent] private ReputationManager _reputationManager;

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _place;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Localize _league;

    public Factory CurrentFactory { get; private set; } = null;

    public void Initialize(Factory factory)
    {
        if (factory == null) return;
        if (CurrentFactory != null)
        {
            CurrentFactory.OnMyFactoryIconChanged -= UpdateIcon;
            CurrentFactory.OnMyFactoryNameChanged -= UpdateName;
            _image.sprite = null;
            _name.text = "Empty";
        }

        CurrentFactory = factory;
        CurrentFactory.OnMyFactoryIconChanged += UpdateIcon;
        CurrentFactory.OnMyFactoryNameChanged += UpdateName;

        _leaderboardManager.OnLeaderboardUpdate += Refresh;

        Refresh();
    }

    public void Refresh()
    {
        if (CurrentFactory == null) return;
        UpdateIcon();
        UpdateName();
        PlaceTextUpdate();
        LeagueTextUpdate();
    }

    private void UpdateIcon() => _image.sprite = CurrentFactory.Icon;

    private void UpdateName()
    {
        if (_name == null) return;

        if (CurrentFactory.IsMyFactory)
        {
            if (_leaderboardManager == null) return;

            if (_leaderboardManager.MyFactory.MyFactoryName == null || 
                _leaderboardManager.MyFactory.MyFactoryName == string.Empty)
                _name.text = "";
            else
                _name.text = _leaderboardManager.MyFactory.MyFactoryName.ToString();
        }
    }

    public void PlaceTextUpdate()
    {
        if (CurrentFactory == null) return;
        _place.text = $"{CurrentFactory.Place}";
    }
    public void LeagueTextUpdate()
    {
        if (CurrentFactory == null) return;
        _league.Term = $"{TranslationKeys.LeaguePrefix}{CurrentFactory.LeagueType}";
    }
}

