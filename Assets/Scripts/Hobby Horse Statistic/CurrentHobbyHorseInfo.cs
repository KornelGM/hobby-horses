using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HH Info", menuName = "ScriptableObjects/Hobby Horse Info/new HH Info")]
public class CurrentHobbyHorseInfo : ScriptableObject
{
    public List<string> PartsGuids => _partsGuids;
    private List<string> _partsGuids = new();

    public void SetPartsGuids(List<string> partsGuids)
    {
        _partsGuids = new(partsGuids);
    }

    public void ClearOnQuit()
    {
        _partsGuids.Clear();
    }
}
