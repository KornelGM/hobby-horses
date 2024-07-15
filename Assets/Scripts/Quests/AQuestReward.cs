using System.Collections;
using Sirenix.OdinInspector;

public abstract class AQuestReward : SerializedScriptableObject
{
    public abstract IEnumerator AwardPlayer();
}
