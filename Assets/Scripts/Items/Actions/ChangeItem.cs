using UnityEngine;
using DG.Tweening;

public class ChangeItem : BaseAction
{
    [SerializeField] private PrefabsContainer _prefabs;
    [SerializeField] private int _currentID;
    [SerializeField] private AnimationCurve _popOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    [SerializeField] private AnimationCurve _popIn = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private float _tweenTime = 0.3f;

    public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller)
    {
        return !_prefabs.IsHighestID(_currentID);
    }

    private void Start()
    {
        _prefabs.IsNotNull(this, nameof(_prefabs));
        ServiceLocator serviceLocator = GetComponentInParent<ServiceLocator>();

        serviceLocator.TryGetServiceLocatorComponent(out VisualItemService visual);
        PopOutAnimation(visual.Model.transform);
    }

    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand)
    {
        GameObject root = (itemInInteractionServiceLocator.gameObject);
        if (root == null) return;

        ServiceLocator serviceLocator = root.GetComponent<ServiceLocator>();
        if (serviceLocator == null) return;

        serviceLocator.TryGetServiceLocatorComponent(out VisualItemService visual);

        GameObject prefabReference = _prefabs.GetPrefab(_prefabs.GetNextID(_currentID));

        PopInAnimation(visual.Model.transform, root, prefabReference);
    }

    private void PopOutAnimation(Transform visualTransform)
    {
        visualTransform.localScale = Vector3.zero;
        visualTransform.DOScale(1, _tweenTime).SetEase(_popIn);
    }

    private void PopInAnimation(Transform visualTransform, GameObject oldGameObject, GameObject newGameObject)
    {
        visualTransform.DOScale(0, _tweenTime).SetEase(_popOut).OnComplete(() =>
        {
            //TODO: Spawning/Destroying through the ItemsManager
            GameObject newPrefab = Instantiate(newGameObject, oldGameObject.transform.position, oldGameObject.transform.rotation);
            Destroy(oldGameObject);
        });
    }
}
