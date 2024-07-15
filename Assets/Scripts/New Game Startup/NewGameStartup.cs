using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Startup", menuName = "ScriptableObjects/New Game/Startup")]
public class NewGameStartup : ScriptableObject
{
    [SerializeField, FoldoutGroup("Default settings")] private string _defaultFactoryName;
    [SerializeField, FoldoutGroup("Default settings")] private int _defaultFactoryIconIndex;
    [SerializeField, FoldoutGroup("Default settings")] private float _defaultFundsMultiplier;
    [SerializeField, FoldoutGroup("Default settings")] private float _defaultReputationMultiplier;

    [SerializeField, ReadOnly] public string FactoryName;
    [SerializeField, ReadOnly] public int FactoryIconIndex;
    [SerializeField, ReadOnly] public float FundsMultiplier;
    [SerializeField, ReadOnly] public float ReputationMultiplier;

    public void SetDefaultVariables()
    {
        FactoryName = _defaultFactoryName;
        FactoryIconIndex = _defaultFactoryIconIndex;
        FundsMultiplier = _defaultFundsMultiplier;
        ReputationMultiplier = _defaultReputationMultiplier;
    }

    public string GetDefaultFactoryName()
    {
        return _defaultFactoryName;
    }
}
