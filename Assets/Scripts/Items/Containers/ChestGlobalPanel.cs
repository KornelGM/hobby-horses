using System.Collections.Generic;
using UnityEngine;

public class ChestGlobalPanel : AItemContainer
{
	[SerializeField] private CharacterHand _playerHand;

	public override void CustomStart()
    {
        base.CustomStart();
        CheckFieldsNull();
        InitializeContainer();
    }

	private void CheckFieldsNull()
	{
		_playerHand.IsNotNull(this, nameof(_playerHand));
	}
}