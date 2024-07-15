using UnityEngine;

public class PlayerLocalizer : Localizer, IServiceLocatorComponent, ISaveable<ServerPlayerSaveData>
{
    [ServiceLocatorComponent] private PlayerSpawnPoint _playerSpawnPoint;
    public ServerPlayerSaveData CollectData(ServerPlayerSaveData data)
    {
        CollectData(data.PlayerLocalization);
        return data;
    }

    public void Initialize(ServerPlayerSaveData save)
    {
        if (save == null)
        {
            if (_playerSpawnPoint == null) return;

            LocalizationSaveInfo localization = new LocalizationSaveInfo(_playerSpawnPoint.transform);
            Initialize(localization);
            return;
        }

        Initialize(save.PlayerLocalization);
    }

    protected override void SetPositionAndRotationOfObject(LocalizationSaveInfo save)
    {
        MyServiceLocator.TryGetComponent(out CharacterController characterController);

        characterController.enabled = false;
        base.SetPositionAndRotationOfObject(save);
        characterController.enabled = true;
    }
}
