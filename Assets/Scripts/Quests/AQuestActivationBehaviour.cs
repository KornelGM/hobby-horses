using System.Collections;
using Sirenix.OdinInspector;

public abstract class AQuestActivationBehaviour : SerializedScriptableObject
{
#if UNITY_EDITOR
    public bool RunInEditor = true;
#endif
    public abstract IEnumerator ExecuteBehaviour();
}
