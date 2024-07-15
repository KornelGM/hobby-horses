using UnityEngine;

[CreateAssetMenu(fileName = "QuestTargetFinder TransformVariable", menuName = "ScriptableObjects/Quests/Target Finders/Transform Variable")]
public class QuestTargetFinderTransformVariable : AQuestTargetFinder
{
    [SerializeField] TransformVariable _transformVariable;

    public override Transform FindObject(ServiceLocator sceneServiceLocator)
    {
        return _transformVariable.Value;
    }
}
