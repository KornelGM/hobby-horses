using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "ScriptableObjects/Items/Color")]
public class ColorHook : ScriptableObject
{
    [field: SerializeField] public int Priority;
    [field: SerializeField, ColorUsage(true, true)] public Color Color;
}
