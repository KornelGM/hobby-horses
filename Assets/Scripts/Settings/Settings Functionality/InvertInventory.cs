using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertInventory : ToggleSetting
{
    [ServiceLocatorComponent(canBeNull: true)] private PlayerManager _playerManager;
    private PlayerQuickAccessItemsBar _playerItemBar;

    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
    }

    public override void Apply()
    {
        base.Apply();

        if(_playerManager == null)
        {
            if(!SceneServiceLocator.Instance.TryGetServiceLocatorComponent(out _playerManager,true))return;
        }
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerItemBar);
        _playerItemBar.InvertInventory = CurrentValue;
    }

}
