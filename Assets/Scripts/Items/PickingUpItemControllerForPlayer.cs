using System.Collections;
using UnityEngine;

public class PickingUpItemControllerForPlayer : PickingUpItemController
{
    [ServiceLocatorComponent] protected CharacterHand _playerHand;
    [ServiceLocatorComponent] protected AItemContainer _quickAccessItemBar;
    
    public AItemContainer QuickAccessItemBar => _quickAccessItemBar;
    
    protected override void Update()
    {
        OnUpdate(_playerHand);
    }
    
    protected override bool TryPickUpItem(ServiceLocator serviceLocator)
    {
        if (_quickAccessItemBar == null) return false;
        
        bool canPickup = _quickAccessItemBar.TryAddItemToContainer(serviceLocator);

        return canPickup;
    }

    protected override void PickUpItem(ServiceLocator serviceLocator)
    {
        if (_quickAccessItemBar == null) return;
        
        _quickAccessItemBar.TryAddItemToContainer(serviceLocator);
    }

    public override void StartPickingUpSmoothed(ServiceLocator item)
    {
        base.StartPickingUpSmoothed(item);
    }
}