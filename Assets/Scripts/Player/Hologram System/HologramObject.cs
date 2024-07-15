using System.Collections.Generic;
using DG.Tweening;
using EPOOutline;
using UnityEngine;

public class HologramObject : MonoBehaviour
{
    public HologramState HologramState { get; private set; }

    [SerializeField] private LayerMask _layerMaskVertical;
    [SerializeField] private float _verticalSpherecastRadius = 0.2f;
    [SerializeField] private LayerMask _layerMaskHorizontal;
    [SerializeField] private Material transparentHologramMaterial;
    [SerializeField] private HologramSettings hologramSettings;

    private GameObject _spaceReservationGameObject;
    private List<Collider> _hologramColliders = new List<Collider>();
    
    [SerializeField] private ColorHook hologramBlocked;
    [SerializeField] private ColorHook hologramPossibleToDrop;
    [SerializeField] private ColorHook hologramPossibleToPlace;

    [SerializeField] private Outlinable _outlinable;

    private ItemHologramSettingsStruct _itemHologramSettings;
    private Transform _playerTransform;

    private void Update()
    {
        DetermineHologramState();
        UpdateHologramMaterial(HologramState);
    }

    public void InitializeHologram(GameObject itemModel, ItemHologramSettingsStruct itemHologramSettingsStruct,
        Transform playerTransform)
    {
        _spaceReservationGameObject = new("Space Reservation Object")
        {
            layer = hologramSettings.IgnoreRaycastLayer
        };
        foreach (var meshFilter in itemModel.GetComponentsInChildren<MeshFilter>())
        {
            GameObject hologramPart = CreateHologramPart(meshFilter);
            if (meshFilter.GetComponents<ItemDetectable>() == null) continue;
            CreateColliderParts(hologramPart, meshFilter.gameObject);
        }


        foreach (var collider in itemModel.GetComponentsInChildren<Collider>())
        {
            if (collider.GetComponent<MeshFilter>() != null) continue;
            GameObject hologramPart = CreateHologramPart(collider.gameObject);
            if (collider.GetComponents<ItemDetectable>() == null) continue;

            CreateColliderParts(hologramPart, collider.gameObject);
        }

        _itemHologramSettings = itemHologramSettingsStruct;
        _playerTransform = playerTransform;
        _spaceReservationGameObject.transform.SetParent(transform);
    }

    public void KillHologram()
    {
        transform
            .DOScale(Vector3.zero, hologramSettings.HologramScalingDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Destroy(_spaceReservationGameObject);
                Destroy(gameObject);
            });
    }

    public void MakeColliderIndependent()
    {
        _spaceReservationGameObject.transform.SetParent(null);
    }

    private GameObject CreateHologramPart(MeshFilter meshFilter)
    {
        GameObject hologramPart = new($"Hologram Model Part");

        hologramPart.transform.CopyTransform(meshFilter.transform);
        hologramPart.layer = hologramSettings.IgnoreRaycastLayer;
        hologramPart.transform.SetParent(transform);
        hologramPart.AddComponent<MeshFilter>().mesh = meshFilter.mesh;

        hologramPart.AddComponent<MeshRenderer>().material = transparentHologramMaterial;

        _outlinable.AddAllChildRenderersToRenderingList();

        return hologramPart;
    }

    private GameObject CreateHologramPart(GameObject colliderObject)
    {
        GameObject hologramPart = new($"Hologram Model Part");
        hologramPart.transform.CopyTransform(colliderObject.transform);
        hologramPart.layer = hologramSettings.IgnoreRaycastLayer;
        hologramPart.transform.SetParent(transform);
        _outlinable.AddAllChildRenderersToRenderingList();
        return hologramPart;
    }

    private void CreateColliderParts(GameObject hologramPart, GameObject meshFilter)
    {
        
        GameObject hologramColliderPart = new("Hologram Collider Part")
        {
            layer = hologramSettings.IgnoreRaycastLayer
        };
        hologramColliderPart.transform.SetParent(_spaceReservationGameObject.transform);
        foreach (var originalCollider in meshFilter.GetComponents<BoxCollider>())
        {
            hologramColliderPart.transform.CopyTransform(hologramPart.transform);
            Collider colliderCopy = (Collider)originalCollider.CopyCollider(hologramColliderPart);
            colliderCopy.isTrigger = true;
            _hologramColliders.Add(colliderCopy);
        }
    }

    private void DetermineHologramState()
    {
        Bounds hologramBounds = new Bounds(transform.position, Vector3.zero);

        foreach (var hologramCollider in _hologramColliders)
        {
            hologramBounds.Encapsulate(hologramCollider.bounds);
        }

        bool isBlockedByWall = IsBlockedByWall();

        Collider[] overlapped = GetOverlappedColliders(hologramBounds);
        List<Collider> surfaceColliders = FindHologramSurfaceColliders(overlapped, hologramBounds);


        if (surfaceColliders.Count > 0 && !isBlockedByWall && !IsOverlappingAnotherCollider(overlapped, hologramBounds, surfaceColliders))
        {
            HologramState = HologramState.PossibleToPlace;
        }
        else if (IsOverlappingAnotherCollider(overlapped, hologramBounds, surfaceColliders))
        {
            HologramState = HologramState.Blocked;
        }
        else
        {
            HologramState = HologramState.PossibleToDrop;
        }
    }

    private bool IsBlockedByWall()
    {
        if (_playerTransform == null) return false;
        Vector3 playerPosition = _playerTransform.position + (1.8f * Vector3.up);
        Ray ray = new Ray(transform.position, playerPosition - transform.position);

        bool hitAnything = Physics.Raycast(ray, Vector3.Distance(transform.position, playerPosition), _itemHologramSettings.IsVertical ? _layerMaskVertical : _layerMaskHorizontal);

        return hitAnything;
    }

    private List<Collider> FindHologramSurfaceColliders(Collider[] overlapped, Bounds hologramBounds)
    {
        List<Collider> surfaceObjects = new();

        List<Collider> surface = _itemHologramSettings.IsVertical ? CheckForVerticalSurface() : CheckForHorizontalSurface();
        if (surface != null)
        {
            surfaceObjects.AddRange(surface);
        }
        return surfaceObjects;
    }

    private List<Collider> CheckForHorizontalSurface()
    {
        List<Collider> surfaceObjects = new();
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (0.1f * transform.up), -transform.up, out hit, 
            hologramSettings.MaxHologramDistanceFromHorizontalSurface * 2, _itemHologramSettings.OverLappingLayerMask))
        {
            if (hit.collider != null)
            {
                surfaceObjects.Add(hit.collider);
            }
        }

        return surfaceObjects;

    }

    private List<Collider> CheckForVerticalSurface()
    {

        List<Collider> surfaceObjects = new();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + transform.forward * hologramSettings.MaxHologramDistanceFromVerticalSurface,
                _verticalSpherecastRadius,
               -transform.forward, hologramSettings.MaxHologramDistanceFromVerticalSurface,
               _itemHologramSettings.OverLappingLayerMask);

        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                surfaceObjects.Add(hit.collider);
            }
        }

        return surfaceObjects;
    }

    private Collider[] GetOverlappedColliders(Bounds hologramBounds)
    {
        if (_hologramColliders == null)
        {
            return new Collider[hologramSettings.MaxOverlappedColliders];
        }

        Collider[] overlapped = new Collider[hologramSettings.MaxOverlappedColliders];
        Physics.OverlapBoxNonAlloc(hologramBounds.center, hologramBounds.extents, overlapped, Quaternion.identity, _itemHologramSettings.IsVertical? _layerMaskVertical : _layerMaskHorizontal);
        return overlapped;
    }

    private bool IsOverlappingAnotherCollider(Collider[] overlapped, Bounds hologramBounds,
        List<Collider> surfaceColliders)
    {
        for (int i = 0; i < hologramSettings.MaxOverlappedColliders; i++)
        {
            if (overlapped[i] != null && !_hologramColliders.Contains(overlapped[i]) &&
                !surfaceColliders.Contains(overlapped[i]))
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateHologramMaterial(HologramState state)
    {
        switch (state)
        {
            case HologramState.Blocked:
            {
                transparentHologramMaterial.color = hologramBlocked.Color;
                _outlinable.OutlineParameters.Color = hologramBlocked.Color;
                break;
            }
            case HologramState.PossibleToDrop:
            {
                transparentHologramMaterial.color = hologramPossibleToDrop.Color;
                _outlinable.OutlineParameters.Color = hologramPossibleToDrop.Color;
                    break;
            }
            case HologramState.PossibleToPlace:
            {
                transparentHologramMaterial.color = hologramPossibleToPlace.Color;
                _outlinable.OutlineParameters.Color = hologramPossibleToPlace.Color;
                break;
            }
            default:
            {
                break;
            }
        }
    }
}