using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HobbyHorsePlayerManager : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private HobbyHorseCustomizationManager _customizationManager;

    public Action<PlayerServiceLocator> OnLocalPlayerSet;
    public PlayerSpawnPoint PlayerSpawnPoint => _playerSpawnPoint;

    public Transform PlayerTransform => LocalPlayer.transform;
    public Vector3 PlayerPosition => LocalPlayer.transform.position;

    public bool Enabled => true;
    [ServiceLocatorComponent] private PlayerSpawnPoint _playerSpawnPoint;

    [SerializeField] private PlayerServiceLocator _playerPrefab;
    public PlayerServiceLocator LocalPlayer { get; private set; }

    public void CustomAwake()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (_playerSpawnPoint == null)
        {
            Debug.LogError($"Add {nameof(PlayerSpawnPoint)} component on scene to determine players spawn point");
            return;
        }

        LocalPlayer = Instantiate(_playerPrefab, _playerSpawnPoint.transform.position, _playerSpawnPoint.transform.rotation);
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        if (!LocalPlayer.TryGetServiceLocatorComponent(out HobbyHorseMovement movement))
            return;

        if (_customizationManager.CurrentHobbyHorseInfo.PartsGuids is { Count: > 0 })
            movement.CustomHobbyHorse.LoadAppearance(_customizationManager.CurrentHobbyHorseInfo.PartsGuids.ToArray());
        else
            movement.CustomHobbyHorse.LoadDefaultAppearance();

        List<HobbyHorseStats> allStats = new();

        foreach (var guid in movement.CustomHobbyHorse.HobbyHorsePartsGuids)
        {
            HobbyHorseCustomizationPartInfo newInfo = _customizationManager.HobbyHorseParts.FirstOrDefault(info => info.Guid == guid);

            if (newInfo == null)
                continue;

            if (!newInfo.HasStats || newInfo.HobbyHorseStats == null)
                continue;

            allStats.Add(newInfo.HobbyHorseStats);
        }

        movement.SetStats(allStats.ToArray());
    }
}
