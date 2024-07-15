using System.Collections.Generic;

public interface IItemInteractionTooltip
{
    string Name { get; }
    ClampedValue ProgressBar { get; }

    Dictionary<InteractionType, IActionTooltip> Actions { get; }
}