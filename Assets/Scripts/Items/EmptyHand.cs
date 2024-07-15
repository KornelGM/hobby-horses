using UnityEngine;

[CreateAssetMenu(fileName = "Empty Hand", menuName = "ScriptableObjects/Items/Empty Hand")]
public class EmptyHand : ItemData
{
	public EmptyHand()
	{
		Id = "EmptyHand";
		Prefab = null;
    }
}