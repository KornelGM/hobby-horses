using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CarryingBox : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private Collider _checkCollider;

    private List<ItemDataContainer> _itemsInBox = new List<ItemDataContainer>();

    private void OnTriggerEnter(Collider other)
    {
        ItemDataContainer item = other.GetComponentInParent<ItemDataContainer>();
        if (item == null) return;
        if (!item.ItemData.CanBeCarryingInBox) return;

        if(!_itemsInBox.Contains(item))
            _itemsInBox.Add(item);

        item.transform.SetParent(_itemsParent);
    }

    private void OnTriggerExit(Collider other)
    {
        ItemDataContainer item = other.GetComponentInParent<ItemDataContainer>();
        if (item == null) return;

        if (item.MyServiceLocator.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
        item.transform.SetParent(null);
        ChangeVisualsLayer(item, "Default");
        _itemsInBox.Remove(item);
    }

    public void OnPickUp()
    {
        _checkCollider.enabled = false;
        foreach (ItemDataContainer item in _itemsInBox)
        {
            if (item.MyServiceLocator.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
            ChangeVisualsLayer(item, "Hand");
        }  
    }

    private void ChangeVisualsLayer(ItemDataContainer item, string layer)
    {
        if (!item.MyServiceLocator.TryGetServiceLocatorComponent(out VisualItemService visualItemService)) return;
        
        Collider[] objectsToChangeLayer = visualItemService.Model.transform.GetComponentsInChildren<Collider>();
        foreach (var itemVisualObject in objectsToChangeLayer)
        {
            itemVisualObject.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

    public void OnDrop()
    {
        foreach (ItemDataContainer item in _itemsInBox)
        {
            ChangeVisualsLayer(item, "Default");
            if (item.MyServiceLocator.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
            }
        }
        _checkCollider.enabled = true;
    }
}
