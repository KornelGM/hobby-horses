using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "DirtInfo ", menuName = "ScriptableObjects/Dirt/DirtInfo")]
public class DirtInfo : SerializedScriptableObject
{
    public readonly string DirtName;
    public readonly float DirtAmount;
}
