using UnityEngine;

public class GlobalSlotsContainer : AItemContainer
{
	[SerializeField] private ChestGlobalPanel _chestGlobalPanel;
	public override void CustomStart()
    {
        base.CustomStart();
        CreateListOfSlots(_amountOfSlots);
    }
}