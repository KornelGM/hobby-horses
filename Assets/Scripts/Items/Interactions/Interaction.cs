using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interaction", menuName = "ScriptableObjects/Items/Interactions")]
public class Interaction : SerializedScriptableObject
{
	public List<InteractionType> InteractionKeys;
	[OdinSerialize] public IItemInteractionTooltip Tooltip;
	[OdinSerialize] public IAction Action;
}