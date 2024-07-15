using UnityEngine;

public abstract class AChest : AItemContainer
{
	public ChestGlobalPanel _chestGlobalPanel;
	
	public override void CustomStart()
    {
        base.CustomStart();
        CreateListOfSlots(_amountOfSlots);
    }
}