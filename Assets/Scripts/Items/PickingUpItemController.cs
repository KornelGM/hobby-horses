using System.Collections;
using UnityEngine;

public abstract class PickingUpItemController : MonoBehaviour, IServiceLocatorComponent
{
    public bool CantPickUp { get; protected set; }

    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] protected float smoothingStep = 2f;
    [SerializeField] protected float distanceToSnapToHand = 0.15f;
    [SerializeField] private float _pickupDelay = 0.15f;

    protected ServiceLocator _itemServiceLocator;
    protected bool _isSmoothing;
    protected float _smoothingTimer;

    protected abstract void Update();
    protected abstract bool TryPickUpItem(ServiceLocator serviceLocator);
    protected abstract void PickUpItem(ServiceLocator serviceLocator);

    public virtual void StartPickingUpSmoothed(ServiceLocator item)
    {
        if (CantPickUp)
            return;

        StartPickUpCounter();

        _itemServiceLocator = item;
        _smoothingTimer = 0;
        _isSmoothing = true;
    }

    protected void StartPickUpCounter()
    {
        CantPickUp = true;
        StartCoroutine(PickupCounter());
    }

    private IEnumerator PickupCounter()
    {
        yield return new WaitForSeconds(_pickupDelay);

        CantPickUp = false;
    }

    protected void OnUpdate(Hand hand)
    {
        if (!_isSmoothing) return;
        if (hand == null) return;

        Vector3 pos;
        Quaternion rotation;
        Transform parent;
        (pos, rotation, parent) = hand.GetHeldDestination(_itemServiceLocator);
        Vector3 localPos = parent.InverseTransformPoint(pos);

        _smoothingTimer += Time.deltaTime;
        _itemServiceLocator.transform.position =
            Vector3.Slerp(_itemServiceLocator.transform.position, parent.TransformPoint(localPos), smoothingStep * (Time.deltaTime + _smoothingTimer));
        _itemServiceLocator.transform.rotation = Quaternion.Slerp(_itemServiceLocator.transform.rotation, rotation, smoothingStep);
        
        if (Vector3.Distance(_itemServiceLocator.transform.position, parent.TransformPoint(localPos)) <= distanceToSnapToHand)
        {
            CancelPickingUpSmoothed();
            TryPickUpItem(_itemServiceLocator);
        }
    }

    private void CancelPickingUpSmoothed() => _isSmoothing = false;
}