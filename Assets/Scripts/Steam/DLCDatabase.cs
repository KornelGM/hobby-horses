using UnityEngine;

[CreateAssetMenu(fileName = "DLC Database", menuName = "ScriptableObjects/Steam/DLC Database")]
public class DLCDatabase : ScriptableObject
{
    public DLCData[] DLCData => _dlcData;
    [SerializeField] private DLCData[] _dlcData;
}
