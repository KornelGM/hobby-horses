using DG.Tweening;
using UnityEngine;


//[RequireComponent(typeof(DropAction))]
public class HologramController : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public bool Projecting { get; set; }
    [field: SerializeField] public UniversalItemDatas UniversalItemDatas { get; private set; }

    [SerializeField] private GameObject hologramObjectPrefab;
    [SerializeField] private Transform hologramObjectParent;
    [SerializeField] private float rotatingHologramSpeed = 180f;
    [SerializeField] private HologramSettings hologramSettings;
    [SerializeField] private ItemHologramSettings defaultItemHologramSettings;

    private InteractableDetector _interactableDetector;
    private PlayerStateMachine _playerStateMachine;
    private CharacterHand _characterHand;
    
    private InteractableSelector _interactableSelector;

    private void UnblockInterectaleSelector() => _interactableSelector.Unblock(this);

    private IAvailableActions _availableActions;
    private ItemData _hologramData;
    private IVirtualController _virtualController;
    private DropAction _dropAction;

    private HologramObject _hologramObject;
    private Transform _hologramTransform;
    private GameObject _hologramGameObject;
    private ItemHologramSettingsStruct _itemHologramSettings;
    private Vector3 _itemPlacingPosition;
    private Vector3 _itemPlacingNormal;

    private float _hologramRotation;
    private bool _isSnappingEnabled = false;

    private void Awake()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out _virtualController);
    }

    private void OnEnable()
    {
        _virtualController.OnToggleHologramGridSnappingPerformed += ToggleHologramGridSnapping;
    }

    private void Start()
    {
        UniversalItemDatas.IsNotNull(this);
        MyServiceLocator.TryGetServiceLocatorComponent(out _interactableDetector);
        MyServiceLocator.TryGetServiceLocatorComponent(out _playerStateMachine);
        MyServiceLocator.TryGetServiceLocatorComponent(out _characterHand);
        MyServiceLocator.TryGetServiceLocatorComponent(out _interactableSelector);
        MyServiceLocator.TryGetServiceLocatorComponent(out _availableActions);

        _hologramData = UniversalItemDatas.HologramObject;
        //_dropAction = GetComponent<DropAction>();

        _itemHologramSettings = defaultItemHologramSettings.ToStruct();
    }

 
    private void Update()
    {
        if(!_itemHologramSettings.IsVertical)
            _hologramRotation += _virtualController.HologramRotatingAxis * rotatingHologramSpeed * Time.deltaTime;
    }

    private void OnDisable()
    {
        _virtualController.OnToggleHologramGridSnappingPerformed -= ToggleHologramGridSnapping;
    }

    public void StartDisplaying(GameObject itemModel,ItemHologramSettings itemHologramSettings)
    {
        _itemHologramSettings = itemHologramSettings != null ? itemHologramSettings.ToStruct() : defaultItemHologramSettings.ToStruct();
        _hologramRotation = 0;
        _interactableSelector.Block(this);
        _availableActions.SetupActions(
            ActionsUtility.ReadActionsBetween(_characterHand.ItemInHand, null, _hologramData),
            _characterHand.ItemInHand, null);

        Projecting = true;
        hologramObjectParent.eulerAngles = new Vector3(90, 0, 0);

        SetupHologram(itemModel, _itemHologramSettings, _playerStateMachine.transform);
    }

    public void UpdateHologram()
    {
        DetermineItemPlacingPosition();
        UpdateHologramTransform();
    }

    public void StopDisplayingHologram()
    {

        if (_characterHand.ItemInHand.TryGetServiceLocatorComponent(out DropAction dropAction, true, false))
        {
            _dropAction = dropAction;
        }
        else
        {
            _dropAction = GetComponent<DropAction>();
        }

        if (_hologramGameObject == null)
        {
            CancelDisplayingHologram();
            return;
        }

        if (_hologramObject.HologramState == HologramState.Blocked)
        {
            CancelDisplayingHologram();
            _interactableSelector.Unblock(this);
            return;
        }

        if (_hologramObject.HologramState == HologramState.PossibleToDrop)
        {
            _dropAction.Perform(_playerStateMachine.MyServiceLocator, null, _characterHand.ItemInHand);
            _interactableSelector.Unblock(this);
            _dropAction.OnEndDropAnimation?.Invoke();
            CancelDisplayingHologram();
            return;
        }


        PlaceItemOnHologram(_characterHand.ItemInHand.gameObject, _itemPlacingPosition,
             hologramSettings.PlacingItemDuration);
        CancelDisplayingHologram();
        _interactableSelector.Unblock(this);
        _dropAction.Perform(MyServiceLocator, null, _characterHand.ItemInHand);
    }

    public void CancelDisplayingHologram()
    {
        Projecting = false;
        if (_hologramObject == null)
        {
            return;
        }

        _hologramTransform.SetParent(null);
        _hologramObject.KillHologram();
        _hologramGameObject = null;
        _hologramTransform = null;
        _hologramObject = null;
    }

    private void SetupHologram(GameObject itemModel, ItemHologramSettingsStruct itemHologramSettingsStruct, Transform playerTransform)
    {
        Projecting = true;
        _hologramGameObject = Instantiate(hologramObjectPrefab);
        _hologramTransform = _hologramGameObject.transform;
        _hologramObject = _hologramGameObject.GetComponent<HologramObject>();
        _hologramTransform.CopyTransform(itemModel.transform);

        _hologramObject.InitializeHologram(itemModel, itemHologramSettingsStruct, playerTransform);

        _hologramTransform.CopyTransform(hologramObjectParent);
        _hologramTransform.SetParent(hologramObjectParent);

        _hologramGameObject.SetActive(true);


        _hologramTransform.localScale = Vector3.zero;

        _hologramTransform
            .DOScale(Vector3.one, hologramSettings.HologramScalingDuration)
            .SetEase(Ease.InOutSine);
    }

    private void DetermineItemPlacingPosition()
    {
        RaycastHit[] detectedItems = _interactableDetector.GetDetectedItemsSorted();
        if (detectedItems != null && detectedItems.Length != 0)
        {
            _itemPlacingPosition = detectedItems[0].point;
            _itemPlacingNormal = detectedItems[0].normal;
        }
        else
        {
            _itemPlacingPosition = _playerStateMachine.FirstPersonCamera.position +
                                   (_playerStateMachine.FirstPersonCamera.forward *
                                    hologramSettings.MaxPlacingItemDistance);
            _itemPlacingNormal = Vector3.zero;
        }

        if (_isSnappingEnabled)
        {
            if (_itemHologramSettings.IsVertical)
            {
                _itemPlacingPosition = new Vector3(
                    Utilities.SnapToGrid(_itemPlacingPosition.x, _itemHologramSettings.SnappingGridSize),
                    Utilities.SnapToGrid(_itemPlacingPosition.y, _itemHologramSettings.SnappingGridSize),
                    Utilities.SnapToGrid(_itemPlacingPosition.z, _itemHologramSettings.SnappingGridSize)
                );
            }
            else
            {
                _itemPlacingPosition = new Vector3(
                    Utilities.SnapToGrid(_itemPlacingPosition.x, _itemHologramSettings.SnappingGridSize),
                    _itemPlacingPosition.y,
                    Utilities.SnapToGrid(_itemPlacingPosition.z, _itemHologramSettings.SnappingGridSize)
                );
            }
        }
    }

    private void UpdateHologramTransform()
    {
        hologramObjectParent.position = _itemPlacingPosition;
        _hologramRotation %= 360;

        if (_itemHologramSettings.IsVertical)
        {
            hologramObjectParent.forward = _itemPlacingNormal;

            if (_isSnappingEnabled)
            {
                hologramObjectParent.RotateAround(hologramObjectParent.position, hologramObjectParent.up,
                    Utilities.SnapToGrid(_hologramRotation, _itemHologramSettings.SnappingRotationAngle));
            }
            else
            {
                hologramObjectParent.RotateAround(hologramObjectParent.position, hologramObjectParent.up,
                    _hologramRotation);
            }
        }
        else
        {
            if (_isSnappingEnabled)
            {
                hologramObjectParent.rotation =
                    Quaternion.AngleAxis(
                        Utilities.SnapToGrid(_hologramRotation, _itemHologramSettings.SnappingRotationAngle),
                        Vector3.up);
            }
            else
            {
                hologramObjectParent.rotation =
                    _characterHand.transform.rotation
                    * Quaternion.AngleAxis(_hologramRotation, Vector3.up);
            }
        }
    }

    private void PlaceItemOnHologram(GameObject obj, Vector3 newItemPlacingPosition,
        float newItemPlacingDuration)
    {
        Transform itemToPlaceTransform = obj.transform;

        itemToPlaceTransform.parent = null;

        if (itemToPlaceTransform != null)
        {
            Vector3 desiredRotation = hologramObjectParent.eulerAngles;

            _hologramObject.MakeColliderIndependent();
            itemToPlaceTransform
                .DORotate(desiredRotation, newItemPlacingDuration)
                .SetEase(Ease.InOutSine);
            itemToPlaceTransform
                .DOJump(newItemPlacingPosition, 0.5f, 1, newItemPlacingDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    UnblockInterectaleSelector();

                    Rigidbody itemRigidbody;
                    obj.TryGetComponent(out itemRigidbody);
                    if (itemRigidbody == null) return;

                    _dropAction.OnEndDropAnimation?.Invoke();
                    HandleRigidbodyAfterPlacement(itemRigidbody);
                });
        }
    }

    private void HandleRigidbodyAfterPlacement(Rigidbody itemRigidbody)
    {
        if (_itemHologramSettings.IsVertical == false)
            return;

        itemRigidbody.useGravity = false;
        itemRigidbody.isKinematic = true;
    }

    private void ToggleHologramGridSnapping()
    {
        _isSnappingEnabled = !_isSnappingEnabled;
    }
}