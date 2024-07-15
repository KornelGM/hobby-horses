using UnityEngine;

[CreateAssetMenu(fileName = "Item Hologram Settings",
    menuName = "ScriptableObjects/Player/Hologram/ItemHologramSettings")]
public class ItemHologramSettings : ScriptableObject
{
    [field: SerializeField] public bool IsVertical { get; private set; } = false;
    [field: SerializeField] public float SnappingGridSize { get; private set; } = 1f;
    [field: SerializeField] public float SnappingRotationAngle { get; private set; } = 90f;
    [field: SerializeField] public LayerMask OverLappingLayerMask { get; private set; }

    public void SetIsVertical(bool isVertical) => IsVertical = isVertical;
    public void SetSnappingGridSize(float gridSize) => SnappingGridSize = gridSize;
    public void SetSnappingRotationAngle(float rotationAngle) => SnappingRotationAngle = rotationAngle;

    public ItemHologramSettingsStruct ToStruct()
    {
        return new()
        {
            IsVertical = IsVertical,
            SnappingGridSize = SnappingGridSize,
            SnappingRotationAngle = SnappingRotationAngle,
            OverLappingLayerMask = OverLappingLayerMask,
        };
    }
}