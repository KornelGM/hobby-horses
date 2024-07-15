using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SmoothingSettings", menuName = "ScriptableObjects/Animations/SmoothingSettings")]
public class MovementSmoothingSettings : SerializedScriptableObject
{
    public float TurnSpeed;
    public AnimationCurve TurnCurve;
}
