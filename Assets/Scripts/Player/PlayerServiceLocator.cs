using System.Collections.Generic;

public class PlayerServiceLocator : ServiceLocator
{
    protected override List<IServiceLocatorComponent> GetNonMonoBehaviourServiceLocators()
    {
        List<IServiceLocatorComponent> components = base.GetNonMonoBehaviourServiceLocators();
        PlayerCurrentActionsReference playerCurrentActionsReference = new PlayerCurrentActionsReference();
        CharacterHandInventorySlot characterHandInventorySlot = new CharacterHandInventorySlot();

        components.Add(characterHandInventorySlot);
        components.Add(playerCurrentActionsReference);
        return components;
    }

    public override void CustomAwake()
    {
        base.CustomAwake();
    }

    protected override void StartComponents()
    {
        base.StartComponents();
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }
}
