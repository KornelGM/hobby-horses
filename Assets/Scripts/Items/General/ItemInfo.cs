using System;
using UnityEngine;

public class ItemInfo : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }

	public event Action<bool> OnItemPickedUpOrDropped;
	
	public bool ItemPickedUp
	{
		get => _itemPickedUp;
		set
		{
			_itemPickedUp = value;
			OnItemPickedUpOrDropped?.Invoke(value);
		}
	}
	
	private bool _itemPickedUp;
}