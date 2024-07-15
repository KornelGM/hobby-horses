using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IManager, ISaveable<SaveData>, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public Action<PlayerServiceLocator> OnLocalPlayerSet;
    public PlayerSpawnPoint PlayerSpawnPoint => _playerSpawnPoint;

    public Transform PlayerTransform => LocalPlayer.transform;
    public Vector3 PlayerPosition => LocalPlayer.transform.position;

    public bool Enabled => true;
    [ServiceLocatorComponent] private PlayerSpawnPoint _playerSpawnPoint;
    [ServiceLocatorComponent] private ItemsManager _itemsManager;

    [SerializeField] private PlayerServiceLocator _playerPrefab;
    public PlayerServiceLocator LocalPlayer { get; private set; }

    private SaveData _loadedData = null;
    public SaveData LoadedData => _loadedData;

    public InventorySaveInfo PlayerInventoryInfo = new();

    public void Initialize(SaveData save)
    {
        SpawnPlayer(save);
        _loadedData = save;
        if (save == null)
        {
            _loadedData = new();
            return;
        }
        DisablePlayersItems(save.PlayerSaveData);
        UpdatePlayerInventoryInfo(save.PlayerSaveData.InventorySaveData);
    }

    public void SpawnPlayer(SaveData save)
    {
        if (_playerSpawnPoint == null)
        {
            Debug.LogError($"Add {nameof(PlayerSpawnPoint)} component on scene to determine players spawn point");
            //return null;
        }

        PlayerServiceLocator player = Instantiate(_playerPrefab, _playerSpawnPoint.transform.position, _playerSpawnPoint.transform.rotation);
        AddPlayer(player);

        if (save == null)
            return;

        SetupPlayer(player, save.PlayerSaveData);
    }

    public SaveData CollectData(SaveData data)
    {
        if (!LocalPlayer.TryGetServiceLocatorComponent(out ISaveable<ServerPlayerSaveData> playerSaveData)) return null;

        playerSaveData.CollectData(data.PlayerSaveData);

        data.PlayerSaveData.PlayerLocalization = new(PlayerTransform);

        data.PlayerSaveData.InventorySaveData = GetPlayerInventorySaveInfo();

        return data;
    }

    public void SetupPlayer(PlayerServiceLocator player, ServerPlayerSaveData saveData)
    {
        if (player == null) return;

        LocalPlayer = player;

     //   LocalPlayer.transform.position = saveData.PlayerLocalization.StartPostion;
        if (player.TryGetServiceLocatorComponent(out PlayerSaveService playerSaveService)) 
            playerSaveService.Initialize(saveData);
    }

    public InventorySaveInfo GetPlayerInventorySaveInfo()
    {
        LocalPlayer.TryGetServiceLocatorComponent(out SelectableItemsContainer inventory);
        PlayerInventoryInfo = inventory.GetInventoryInfo();
        return PlayerInventoryInfo;
    }

    public void CustomReset()
    {
        _playerSpawnPoint = null;
        if (Application.isPlaying) Destroy(LocalPlayer);
        else DestroyImmediate(LocalPlayer);
    }

    public void AddPlayer(PlayerServiceLocator serviceLocator)
    {
        LocalPlayer = serviceLocator;
        OnLocalPlayerSet?.Invoke(LocalPlayer);
        return;
    }

    public void HandleLocalPlayer(Action<PlayerServiceLocator> onLocalPlayerSet)
    {
        if (LocalPlayer == null) OnLocalPlayerSet += onLocalPlayerSet;
        else onLocalPlayerSet.Invoke(LocalPlayer);
    }

    private void DisablePlayersItems(ServerPlayerSaveData playersData)
    {
        foreach (ItemSlotSaveInfo slot in playersData.InventorySaveData.InventoryItemSlotSaveInfos)
        {
            _itemsManager.DisableItems(slot.Guids);
        }
    }

    public void UpdatePlayerInventoryInfo(InventorySaveInfo inventorySaveInfo)
    {
        PlayerInventoryInfo = inventorySaveInfo;
    }
}
