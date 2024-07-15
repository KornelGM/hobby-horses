using UnityEngine;

public class TweenAnimationTarget : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public Transform MyTransform => transform;
}
