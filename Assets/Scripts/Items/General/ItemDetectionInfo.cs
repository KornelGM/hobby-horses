using System.Collections.Generic;
using UnityEngine;

public class ItemDetectionInfo : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    public bool AbleToDetect => !_isHolding;
    public ShowItemData ShowItemData => _showItemData;

    private ShowItemData _showItemData;
   public bool _isHolding = false;

    public void CustomAwake()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out _showItemData);
    }

    public void BlockDetection()
    {
        _isHolding = true;
    }

    public void UnblockDetection()
    {
        _isHolding = false;
    }
}
