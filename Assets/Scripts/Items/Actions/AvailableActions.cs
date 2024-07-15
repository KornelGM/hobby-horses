using System.Collections.Generic;

public class AvailableActions 
{
    public Dictionary<InteractionType, List<IAction>> Interactions { get; private set; } = new();

    public AvailableActions() { }
    public AvailableActions(Dictionary<InteractionType, List<IAction>> actions)
    {
        Interactions = actions;
    }
    
    public AvailableActions(List<AvailableActions> actions)
    {
        foreach (var action in actions)
        {
            Add(action);
        }
    }

    private void Add(AvailableActions actions)
    {
        foreach (var pair in actions.Interactions)
        {
            if (Interactions.ContainsKey(pair.Key))
            {
                Interactions[pair.Key].AddRange(pair.Value);
            }
            else
            {
                Interactions[pair.Key] = new List<IAction>(pair.Value);
            }
        }
    }
}
