using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestActivation Set Objects Active", menuName = "ScriptableObjects/Quests/Activation Behaviours/Set Objects Active")]
public class QuestActivationSetObjectsActive : AQuestActivationBehaviour
{
    [SerializeField] TransformVariable[] _objectsToActivate;
    [SerializeField] TransformVariable[] _objectsToDeactivate;

    public override IEnumerator ExecuteBehaviour()
    {
        foreach (TransformVariable variable in _objectsToActivate)
            variable.Value.gameObject.SetActive(true);

        foreach (TransformVariable variable in _objectsToDeactivate)
            variable.Value.gameObject.SetActive(false);

        yield break;
    }
}