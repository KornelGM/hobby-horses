using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionInfo : MonoBehaviour, IServiceLocatorComponent, IAwake
{
	public Transform InteractionsAsCallerParent => _interactionsAsCallerParent;
	public Transform InteractionsWithCallerParent => _interactionsWithCallerParent;
	public List<InteractionContainer> InteractionsAsCaller => _interactionsAsCaller;
	public List<InteractionContainer> InteractionsWithCaller => _interactionsWithCaller;
	
    [SerializeField] private Transform _interactionsAsCallerParent;	
	[SerializeField] private Transform _interactionsWithCallerParent;	
	[SerializeField] private ItemData _everyObjectData;	
	public ServiceLocator MyServiceLocator { get; set; }
	
	private List<InteractionContainer> _interactionsAsCaller;
	private List<InteractionContainer> _interactionsWithCaller;
	private ItemData _myItemData;

	public void CustomAwake()
	{
		if(MyServiceLocator.TryGetServiceLocatorComponent(out ItemDataContainer dataContainer))
		{
			_myItemData = dataContainer.ItemData;
        }

		_interactionsAsCaller = _interactionsAsCallerParent.GetComponentsInChildren<InteractionContainer>().ToList();
		_interactionsWithCaller = _interactionsWithCallerParent.GetComponentsInChildren<InteractionContainer>().ToList();
	}

	public AvailableActions GetPossibleActionsWithObject([CanBeNull] ItemData itemData, [CanBeNull] InteractionInfo info)
    {
		InteractionContainer[] containers = GetInteractionContainersWithItem(itemData, info);
		Dictionary<InteractionType, List<IAction>> actions = new();

        for (int i = 0; i < Enum.GetNames(typeof(InteractionType)).Length; i++)
        {
			InteractionType interactionType = (InteractionType)i;
			actions.Add(interactionType, new());

			foreach (InteractionContainer container in containers)
            {
				if (container == null) continue;
				actions[interactionType].AddRange(container.GetActionsAccordingToType(interactionType));
            }
        }

		return new AvailableActions(actions);
    }

	private InteractionContainer[] GetInteractionContainersWithItem([CanBeNull] ItemData itemData, [CanBeNull] InteractionInfo info)
	{
		InteractionContainer callerWithItem = FindCallerInteractionContainerWith(itemData);

		InteractionContainer itemWithCaller = info?.FindInteractionContainerWithCaller(_myItemData);
		InteractionContainer itemWithEvery = info?.FindInteractionContainerWithCaller(_everyObjectData);

		InteractionContainer calleryWithEvery = FindCallerInteractionContainerWith(_everyObjectData);
		InteractionContainer callerWithSelf = FindCallerInteractionContainerWith(null);

		InteractionContainer[] containers = new InteractionContainer[]
		{
			callerWithItem,

			itemWithCaller,
			itemWithEvery,

			calleryWithEvery,
			callerWithSelf,
		};

		return containers;
	}

	public AvailableActions GetActionsWithSelf()
    {
		return GetPossibleActionsWithObject(null, null);
	}

	public InteractionContainer FindInteractionContainerWithCaller(ItemData held) => FindInteractionContainer(held, _interactionsWithCaller);
	private InteractionContainer FindCallerInteractionContainerWith([CanBeNull] ItemData focused) => FindInteractionContainer(focused, _interactionsAsCaller);	

	private InteractionContainer FindInteractionContainer([CanBeNull] ItemData itemData, List<InteractionContainer> containers)
	{
		foreach (InteractionContainer interactionContainer in containers)
		{
			if (interactionContainer.ItemData != itemData) continue;
			return interactionContainer;
		}

		return null;
	}
}