using System;
using UnityEngine;

public class HobbyHorsePlayerManager : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

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
        //SetupPlayer(player);
    }
}
