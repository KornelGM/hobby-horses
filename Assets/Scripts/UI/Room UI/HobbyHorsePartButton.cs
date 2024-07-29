using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HobbyHorsePartButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CustomizationUI _customizationUI;
    [ServiceLocatorComponent] private HobbyHorseCustomizationManager _customizationManager;

    [SerializeField, FoldoutGroup("References")] private Image _icon;
    [SerializeField, FoldoutGroup("References")] private TextMeshProUGUI _cost;
    [SerializeField, FoldoutGroup("References")] private HobbyHorseStatsUIPanel _statsPanel;

    private HobbyHorseCustomizationPartInfo _partInfo;

    private void OnEnable()
    {
        _customizationManager.OnPartUnlocked += UpdateCost;
    }

    private void OnDisable()
    {
        _customizationManager.OnPartUnlocked -= UpdateCost;
    }

    public void Initialize(HobbyHorseCustomizationPartInfo partInfo)
    {
        if (partInfo == null)
            return;

        _partInfo = partInfo;

        _icon.sprite = _partInfo.Icon;
        UpdateCost();

        _statsPanel.gameObject.SetActive(_partInfo.HasStats);
        if (_partInfo.HasStats)
            _statsPanel.SetStats(_partInfo.HobbyHorseStats);
    }

    private void UpdateCost()
    {
        _cost.text = _customizationManager.CheckHobbyHorsePartIsUnlocked(_partInfo.Guid)
            ? $"Unlocked"
            : $"{_partInfo.Cost}$";
    }

    public void OnButtonDown()
    {
        _customizationUI.SetHobbyHorsePart(_partInfo);
    }

}
