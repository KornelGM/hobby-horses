using UnityEngine;
using Sirenix.OdinInspector;

public class TemporaryQuestStarter : SerializedMonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] IQuestManager _questManager;
    [SerializeField] Quest _startQuest;

    public ServiceLocator MyServiceLocator { get; set; }

    private void Update()
    {
        _questManager.IsNotNull(this, nameof(_questManager));
        _startQuest.IsNotNull(this, nameof(_startQuest));

        _questManager.AddQuest(_startQuest, true);
        enabled = false;
    }
}
