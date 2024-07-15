using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save Service Variable", menuName = "ScriptableObjects/Variables/SaveServiceVariable")]
public class SaveServiceVariable : ScriptableVariable<ISaveable<SaveData>>
{
}
