using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HobbyHorseStatUI : MonoBehaviour
{
    public HobbyHorseStat HobbyHorseStat => _hobbyHorseStat;

    [SerializeField, FoldoutGroup("Stat Type")] private HobbyHorseStat _hobbyHorseStat;
    [SerializeField, FoldoutGroup("Texts")] private TextMeshProUGUI _statNameText;
    [SerializeField, FoldoutGroup("Texts")] private TextMeshProUGUI _statValueText;

    private float _statValue;

    private void Start()
    {
        _statNameText.text = _hobbyHorseStat.ToString();
        _statValue = 0;
    }

    public void SetStatValue(float value)
    {
        _statValue = value;
        UpdateStatTexts();
    }

    private void UpdateStatTexts()
    {
        _statValueText.text = _statValue.ToString();
    }
}
