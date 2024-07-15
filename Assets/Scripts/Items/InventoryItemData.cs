using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item Data", menuName = "ScriptableObjects/Items/Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
	public string Name;
	public Sprite Icon;
	[Range(1, 100)]public int Amount;
}

