using UnityEngine;

public class ItemDetectable : MonoBehaviour, IServiceLocatorComponent, IAwake
{
	public ServiceLocator MyServiceLocator { get; set; }
	public bool AbleToDetect => _info.AbleToDetect && enabled;
	public ShowItemData ShowItemData => _info.ShowItemData;
	public Transform ItemInteractableTransform => MyServiceLocator.transform;

	private ItemDetectionInfo _info;
	
	public void CustomAwake()
	{
		MyServiceLocator.TryGetServiceLocatorComponent(out _info);
	}
}