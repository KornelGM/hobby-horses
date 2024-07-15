using System.Collections.Generic;

public class ItemInteractionTooltip : IItemInteractionTooltip
{
    public string Name { get; private set; }
    public ClampedValue ProgressBar { get; }
    public Dictionary<InteractionType, IActionTooltip> Actions { get; private set; }

    public ItemInteractionTooltip(ClampedValue progressBar)
    {
        Name = string.Empty;
        Actions = null;
        ProgressBar = progressBar;
    }

    public ItemInteractionTooltip(string name, Dictionary<InteractionType, IActionTooltip> allActions)
    {
        Name = name;
        Actions = allActions;
        ProgressBar = null;
    }

    public ItemInteractionTooltip(string name, Dictionary<InteractionType, IActionTooltip> allActions, ClampedValue progressBar)
    {
        Name = name;
        Actions = allActions;
        ProgressBar = progressBar;
    }
}