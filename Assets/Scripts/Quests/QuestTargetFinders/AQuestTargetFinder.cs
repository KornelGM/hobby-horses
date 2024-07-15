using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AQuestTargetFinder : ScriptableObject
{
    public float OffsetY => _offsetY;
    [SerializeField, FoldoutGroup("On Finder Not Found")] public List<AQuestReward> OnFinderNotFound;
    [SerializeField, FoldoutGroup("Offset")] private float _offsetY = 0.5f;
    public virtual Transform FindObject(ServiceLocator sceneServiceLocator)
    {
        return null;
    }

    public virtual Transform FindObject(ServiceLocator sceneServiceLocator, out ServiceLocator targetServiceLocator) 
    {
        targetServiceLocator = null;

        return null;
    }

    public virtual void Activate()
    {
        Activated = true;
    }

    public virtual void Deactivate()
    {
        Activated = false;
    }

    public IEnumerator FinderNotFound(MonoBehaviour emptyMonoBehaviour)
    {
        foreach (AQuestReward qr in OnFinderNotFound)
        {
            yield return emptyMonoBehaviour.StartCoroutine(qr.AwardPlayer());
        }
    }

    public bool Activated { get; private set; }

}
