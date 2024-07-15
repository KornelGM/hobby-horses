using UnityEngine;

[CreateAssetMenu(fileName = "Universal Item Datas", menuName = "ScriptableObjects/Items/Universal Item Datas")]
public class UniversalItemDatas : ScriptableObject
{
    [field:SerializeField] public ItemData EveryObject { get; private set; }
    [field:SerializeField] public ItemData HologramObject { get; private set; }
}
