using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class InteractionContainer : MonoBehaviour
{
	public ItemData ItemData;

	public List<BaseAction> PrimaryInteraction;
	public List<BaseAction> CancelPrimaryInteraction;
	public List<BaseAction> SecondaryInteraction;
	public List<BaseAction> CancelSecondaryInteraction;
	public List<BaseAction> AdditiveInteraction;
	public List<BaseAction> CancelAdditiveInteraction;
    public List<BaseAction> MoreInfoInteraction;
    public List<BaseAction> CancelMoreInfoInteraction;

    public List<BaseAction> GetActionsAccordingToType(InteractionType interactionType)
	{
		switch (interactionType)
		{
			case InteractionType.PrimaryInteraction: return PrimaryInteraction;
			case InteractionType.SecondaryInteraction: return SecondaryInteraction;
			case InteractionType.AdditiveInteraction: return AdditiveInteraction;
			case InteractionType.MoreInfo: return MoreInfoInteraction;
			case InteractionType.CancelPrimaryInteraction: return CancelPrimaryInteraction;
			case InteractionType.CancelSecondaryInteraction: return CancelSecondaryInteraction;
			case InteractionType.CancelAdditiveInteraction: return CancelAdditiveInteraction;
			case InteractionType.CancelMoreInfo: return CancelMoreInfoInteraction;
			default: return PrimaryInteraction;
		}
	}
}