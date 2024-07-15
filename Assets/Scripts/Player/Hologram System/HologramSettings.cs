using UnityEngine;

[CreateAssetMenu(fileName = "Hologram Settings", menuName = "ScriptableObjects/Player/Hologram/HologramSettings")]
public class HologramSettings : ScriptableObject
{
    public int IgnoreRaycastLayer { get; private set; } = 2;
    [field: SerializeField] public float MaxPlacingItemDistance { get; private set; } = 4f;
    [field: SerializeField] public float PlacingItemDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float HologramScalingDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float MaxHologramDistanceFromHorizontalSurface { get; private set; } = 0.05f;
    [field: SerializeField] public float MaxHologramDistanceFromVerticalSurface { get; private set; } = 0.20f;

    [field: SerializeField] public int MaxOverlappedColliders { get; private set; } = 15;
    }