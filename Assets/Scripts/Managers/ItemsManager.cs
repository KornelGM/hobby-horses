
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemsManager : MonoBehaviour, IServiceLocatorComponent, IManager, ISaveable<SaveData>
{
    public Action<int, ItemServiceLocator> OnClientItemReceived;

    public ItemDataDatabase ItemDataDatabase => _dataDatabase;

    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent(canBeNull: true)] private PlayerManager _playerManager;

    public List<ItemServiceLocator> Items
    {
        get
        {
            List<ItemServiceLocator> output = _items.Values.ToList();
            output.AddRange(_itemsWithoutGUID);

            return output;
        }
    }

    [SerializeField] private ItemDataDatabase _dataDatabase;
    private Dictionary<string, ItemServiceLocator> _items = new();
    private List<ItemServiceLocator> _itemsWithoutGUID = new();
    private List<string> _destroyedStaticObjects = new();

    public ItemServiceLocator SpawnItem(ItemData data, Vector3 position, Quaternion rotation, ItemSaveData itemSaveData = null)
    {
        if (data.Prefab == null)
        {
            Debug.LogError($"Object with name {data.name} has no prefab");
            return null;
        }

        ServiceLocator obj = Instantiate(data.Prefab, position, rotation);

        ItemServiceLocator item = obj as ItemServiceLocator;
        if (item == null) return null;

        item.CustomAwake();
        InitializeItem(item, itemSaveData);
        return item;
    }

    public void SpawnItem(ItemSaveData itemSaveData)
    {
        if (InPlayersEquipement(itemSaveData.PersonalGUID))
        {
            if (!PlayerExists()) return;
        }

        SpawnItem(_dataDatabase.GetEntry(itemSaveData.ItemDataGUID),
            itemSaveData.Localization.StartPostion.Vector3,
            itemSaveData.Localization.StartRotation.Quaternion,
            itemSaveData);
    }

    public void HandleItemsInEquipment(IInventoryInfo inventoryInfo)
    {
        if (inventoryInfo == null) return;
        foreach (ItemSlotSaveInfo slot in inventoryInfo.InventoryItemSlotSaveInfos)
        {
            foreach (string guid in slot.Guids)
            {
                if (!GetItemByGUID(guid, out ItemServiceLocator item)) continue;
                PickupOnServer(item);

                bool itemInHand = inventoryInfo.InventoryItemSlotSaveInfos.IndexOf(slot) == inventoryInfo.SelectedSlot;

                if (itemInHand) { item.HideServiceLocator(false); }
                else { item.HideServiceLocator(); }
            }
        }
    }

    private void PickupOnServer(ItemServiceLocator obj)
    {
        Rigidbody rb;
        if (obj.TryGetComponent(out rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (obj.TryGetServiceLocatorComponent(out VisualItemService visualItemService, true, true))
        {
            ChangeVisualsLayer(visualItemService);
        }
    }

    private void ChangeVisualsLayer(VisualItemService visualItemService)
    {
        Transform[] children = visualItemService.gameObject.GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child.GetComponent<DontChangeLayerOnThisItem>())
                continue;

            child.gameObject.layer = LayerMask.NameToLayer("Hand");
        }

    }

    public bool InPlayersEquipement(string guid)
    {
        return true;
    }

    public bool PlayerExists()
    {
        return true;
    }

    public void DisableItems(List<string> guids)
    {
        foreach (string guid in guids)
        {
            if (!GetItemByGUID(guid, out ItemServiceLocator item)) continue;
            item.HideServiceLocator();
        }
    }

    public void InitializeItem(ItemServiceLocator item, ItemSaveData itemSaveData = null)
    {
        if (item.TryGetServiceLocatorComponent(out ItemSaveService itemSaveService, true, false))
        {
            if (!_items.ContainsKey(itemSaveService.GUID))
            {
                itemSaveService.Initialize(itemSaveData);
                _items.Add(itemSaveService.GUID, item);
            }
        }
        else
        {
            if (!_itemsWithoutGUID.Contains(item))
                _itemsWithoutGUID.Add(item);
        }
    }

    public void RemoveItem(ItemServiceLocator item)
    {
        if (item == null) return;
        if (item.gameObject == null) return;

        if (item.TryGetComponent(out ItemSaveService saveService))
        {
            if (saveService.StaticOnScene) _destroyedStaticObjects.Add(saveService.GUID);
            _items.Remove(saveService.GUID);
        }
        else
        {
            _itemsWithoutGUID.Remove(item);
        }
    }

    public SaveData CollectData(SaveData data)
    {
        foreach (ItemServiceLocator item in _items.Values)
        {
            try
            {
                if (item == null) continue;
                if (!item.TryGetServiceLocatorComponent(out ItemSaveService itemSaveService, true, false)) continue;

                ItemSaveData itemSaveData = itemSaveService.CollectData(null);

                if (itemSaveData == null) continue;

                if (itemSaveService.StaticOnScene) data.ItemsOnScene.Items.Add(itemSaveData);
                else data.ItemsSaveData.Items.Add(itemSaveData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        data.ItemsOnScene.DestroyedItems = new(_destroyedStaticObjects);

        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null) return;

        _destroyedStaticObjects = new(save.ItemsOnScene.DestroyedItems);
        HandleObjectsOnScene(save);
    }

    public void LoadClientInventory(InventorySaveInfo inventorySaveInfo)
    {
        foreach (ItemSlotSaveInfo itemSlotInfo in inventorySaveInfo.InventoryItemSlotSaveInfos)
        {
            if (itemSlotInfo.Guids == null) continue;

            foreach (string Guid in itemSlotInfo.Guids)
            {
                if (!GetItemByGUID(Guid, out ItemServiceLocator item)) continue;

                TargetReceiveItem(item, itemSlotInfo.SlotID);
            }
        }
    }

    public void TargetReceiveItem(ItemServiceLocator item, int slotID)
    {
        OnClientItemReceived?.Invoke(slotID, item);
    }

    public bool GetItemByGUID(string guid, out ItemServiceLocator item)
    {
        if (!_items.ContainsKey(guid))
        {
            item = null;
            return false;
        }

        item = _items[guid];
        return true;
    }

    private void SpawnObjects(List<ItemSaveData> itemSaveDatas)
    {
        foreach (ItemSaveData itemData in itemSaveDatas)
        {
            try
            {
                SpawnItem(itemData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private void HandleObjectsOnScene(SaveData save)
    {
        Debug.Log("Initialize");
        //List<ItemServiceLocator> items = _items.Values.ToList();
        ItemServiceLocator[] items = FindObjectsOfType<ItemServiceLocator>(true);

        for (int i = items.Length - 1; i >= 0; i--)
        {
            //ItemServiceLocator item = _items.Values.ToArray()[i];
            ItemServiceLocator item = items[i];

            try
            {
                //We want to handle only saveable objects, for don't duplicating during spawning new ones 
                //Static objects are not spawning during loading game so we are loading it's data directly on scene
                if (!item.TryGetServiceLocatorComponent(out ItemSaveService itemSaveService, true, false)) continue;

                if (itemSaveService.StaticOnScene)
                {
                    if (save == null) continue;
                    if (save.ItemsOnScene == null) continue;
                    if (save.ItemsOnScene.DestroyedItems.Contains(itemSaveService.GUID))
                    {
                        Destroy(item.gameObject);
                        continue;
                    }

                    ItemSaveData itemSaveData = save.ItemsOnScene.Items.Find(saveData => saveData.PersonalGUID == itemSaveService.GUID);

                    TryLoadSceneObjectInfo(itemSaveService, itemSaveData);
                    continue;
                }

                Destroy(item.gameObject);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        StartCoroutine(DelaySpawnItems(save));
    }

    private IEnumerator DelaySpawnItems(SaveData save)
    {
        yield return new WaitForEndOfFrame();
        SpawnObjects(save.ItemsSaveData.Items);

        LoadClientInventory(_playerManager.PlayerInventoryInfo);
        HandleItemsInEquipment(_playerManager.PlayerInventoryInfo);
    }

    private void TryLoadSceneObjectInfo(ItemSaveService item, ItemSaveData itemSaveData)
    {
        if (item == null) return;
        if (itemSaveData == null) return;

        InitializeItem(item.MyServiceLocator as ItemServiceLocator, itemSaveData);
    }
}
