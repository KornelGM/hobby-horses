using System;
using UnityEngine;

using UnityEngine.Serialization;

public abstract class Hand : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator ItemInHand => _itemInHand;
    public bool IsHandEmpty => _itemInHand == _emptyHandServiceLocator;
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] protected ServiceLocator _emptyHandServiceLocatorPrefab;
    [SerializeField] protected Transform _camera;
    [SerializeField] protected ItemData _emptyHand;
    [SerializeField] protected BoxCollider _feetCollider;

	public ItemData ItemDataInItemInHand;

    private ServiceLocator _emptyHandServiceLocator;
    private ServiceLocator _itemInHand;

	public virtual void CustomAwake()
	{
		_emptyHandServiceLocatorPrefab.IsNotNull(this, nameof(_emptyHandServiceLocatorPrefab));
        _emptyHandServiceLocator = Instantiate(_emptyHandServiceLocatorPrefab);
        Instantiate(_emptyHandServiceLocator.gameObject);
    }

    public virtual void OnDestroy() { }

    public void RefreshItemInHandPosition()
    {
        Transform itemInHandTransform = _itemInHand.transform;
        
        itemInHandTransform.localPosition = Vector3.zero;
        itemInHandTransform.localRotation = Quaternion.identity;
    }

    public void ItemInHandIsNull()
    {
        ChangeItemInHand(_emptyHandServiceLocator);
        Debug.LogError("Item in hand shouldn't be null. Something went wrong");
    }

    public virtual (Vector3, Quaternion, Transform) GetLocalHandDestination(ServiceLocator item)
    {
        Vector3 pos;
        Quaternion rotation;
        Transform parent;

        if (item.TryGetServiceLocatorComponent(out ItemDataContainer container) &&
            container.ItemData != null)
        {
            parent = container.ItemData.AttachedToCamera ? _camera : transform;

            pos = container.ItemData.Offset;
            rotation = Quaternion.Euler(container.ItemData.Rotation);
            _feetCollider.center = new Vector3(_feetCollider.center.x,
                _feetCollider.center.y,
                container.ItemData.CenterZ);
        }
        else
        {
            parent = transform;

            pos = new Vector3(0.5f, 1.5f, 1f);
            rotation = Quaternion.identity;
            _feetCollider.center = Vector3.zero;
        }

        return (pos, rotation, parent);
    }

    public virtual (Vector3, Quaternion, Transform) GetHeldDestination(ServiceLocator item)
    {
        Vector3 pos;
        Quaternion rotation;
        Transform parent;

        parent = transform;

        pos = new Vector3(0.5f, 1.5f, 1f);
        rotation = Quaternion.identity;
        _feetCollider.center = Vector3.zero;

        if (item.TryGetServiceLocatorComponent(out ItemDataContainer container, true))
        {
            if(container.ItemData != null)
            {
                parent = container.ItemData.AttachedToCamera ? _camera : transform;

                pos = parent.TransformPoint(container.ItemData.Offset);
                rotation = parent.rotation * Quaternion.Euler(container.ItemData.Rotation);
                _feetCollider.center = new Vector3(_feetCollider.center.x,
                    _feetCollider.center.y,
                    container.ItemData.CenterZ);
            }
        }

        return (pos, rotation, parent);
    }

    protected void OnItemInSlotRefresh(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null)
        {
            ChangeItemInHand(_emptyHandServiceLocator);
            return;
        }

        ChangeItemInHand(serviceLocator);  
    }

    protected void OnItemHidden(ServiceLocator serviceLocator)
    {
        serviceLocator.gameObject.SetActive(false);
    }

    protected void ChangeItemInHand(ServiceLocator newItem)
	{
        if (newItem == null) return;

        if (_itemInHand != null)
        {
            UnblockItem(_itemInHand);
            DetachChildren(_itemInHand.transform);
        }

        _itemInHand = newItem;
        
        if (newItem != null)
        {
            BlockItem(newItem);
            ReParentItem(newItem);
            newItem.gameObject.SetActive(true);
        }

        OnNewItemPlacedInHand(newItem);    
    }

	private void DetachChildren(Transform item)
	{
        item.SetParent(null);
    }

    private void ReParentItem(ServiceLocator item)
    {
        if (item == null) return;

        Vector3 pos;
        Quaternion rotation;
        Transform parent;

        (pos, rotation, parent) = GetHeldDestination(item);
        item.transform.position = pos;
        item.transform.rotation = rotation;
        item.transform.SetParent(parent);
    }

    protected virtual void OnNewItemPlacedInHand(ServiceLocator item)
    {
        item.TryGetServiceLocatorComponent(out ItemDataContainer itemDataContainer);
        ItemDataInItemInHand = itemDataContainer.ItemData;
    }

    public void DestroyItemInHand()
    {
        ChangeItemInHand(_emptyHandServiceLocator);       
        Destroy(_itemInHand.gameObject);
    }

    private static void UnblockItem(ServiceLocator item) 
    {
        if (item.TryGetServiceLocatorComponent(out ItemDetectionInfo info, true))
        {
            info.UnblockDetection();
        }
    }

    private static void BlockItem(ServiceLocator item)
    {
        if (item.TryGetServiceLocatorComponent(out ItemDetectionInfo info, true))
        {
            info.BlockDetection();
        }
    }

}
