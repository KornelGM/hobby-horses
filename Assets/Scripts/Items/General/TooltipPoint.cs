using UnityEngine;

public class TooltipPoint : MonoBehaviour, ITooltipPoint, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public Vector3 Point => gameObject.transform.position;

    public void UpdatePosition(){}
}
