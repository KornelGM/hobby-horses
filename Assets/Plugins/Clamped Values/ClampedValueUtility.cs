using UnityEngine;

static public class ClampedValueUtility
{
    public static void Clamp(this ClampedValue clampedValue)
    {
        clampedValue.Value = Mathf.Clamp(clampedValue.Value, clampedValue.MinValue, clampedValue.MaxValue);
    }
}