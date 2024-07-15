using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private StatsManager _statsManager;

    [SerializeField] private StatsDatabase _statsDatabase;
    [SerializeField] private PlayerStatPanel _playerStatPanelPref;
    [SerializeField] private Transform _layout;

    private List<PlayerStatPanel> _playerStatPanels = new List<PlayerStatPanel>();

    private void OnEnable() => Show();

    private void OnDisable() => Hide();

    public void Show()
    {
        ClearPanels();
        SpawnPanels(_statsManager.Statistics);

        _statsManager.OnStatisticAdded += OnStatisticAdded;
    }

    public void Hide()
    {
        ClearPanels();
        _statsManager.OnStatisticAdded -= OnStatisticAdded;
    }

    private void ClearPanels()
    {
        for (int i = _playerStatPanels.Count-1; i >= 0 ; i--)
        {
            _playerStatPanels.RemoveAt(i);
            Destroy(_playerStatPanelPref.gameObject);
        }
    }

    private void OnStatisticAdded(ActionStat obj)
    {
        
    }

    private void SpawnPanels(Dictionary<string, List<AActionStatData>> stats)
    {
        foreach (var stat in stats)
        {
            SpawnPanel(stat.Key, stat.Value.Count);
        }
    }

    private void SpawnPanel(string guid, int count)
    {
        PlayerStatPanel statPanel = Instantiate(_playerStatPanelPref, _layout);
        ActionStat stat = _statsDatabase.GetEntry(guid);

        if(stat != null)
            statPanel.SetupTxt($"{stat.name} {count}");
        else 
            statPanel.SetupTxt($"{guid} {count}");

    }
}
