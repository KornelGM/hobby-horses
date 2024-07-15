using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "VisualDebugData", menuName = "ScriptableObjects/Debug/VisualDebug/VisualDebugData")]
public class VisualDebugData : SerializedScriptableObject
{
    public string parentCard;

}
