using System.Collections.Generic;
using UnityEngine;

public class ItemVisualDebugger : VisualDebugger
{
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private ItemsManager _itemsManager;
    [SerializeField] private ItemDataDatabase _itemDatabase;
    [SerializeField] List<ItemData> _items = new();

    private ServiceLocator _player;


    public override void CustomStart()
    {
        base.CustomStart();

        _player = _playerManager.LocalPlayer;

        for (int i = 0; i < _itemDatabase.EntryList.Count; i++)
        {
            int j = i;
            var item = _itemDatabase.EntryList[j];

            if(item.Type != ItemType.None && item.Type != ItemType.EmptyHand)
                AddButton(this, (b) => SpawnItemFromDatabase(j), cardPath: $"{item.Type.ToString()}", 
                buttonName: $"Spawn {item.GetItemName()}");
        }

        for (int i = 0; i < _items.Count; i++)
        {
            int j = i;
            AddButton(this, (b) => SpawnItem(j), cardPath: "Other", buttonName: $"Spawn {_items[j].name}");
        }
    }

    public void SpawnItemFromDatabase(int item)
    {
        SpawnItemFromDatabaseCMD(item, _player.transform.position + Vector3.up * 2);
    }

    public void SpawnItem(int item)
    {
        SpawnItemCMD(item, _player.transform.position + Vector3.up * 2);
    }

    public void SpawnItemCMD(int index, Vector3 place)
    {
        _itemsManager.SpawnItem(_items[index], place, Quaternion.identity);
    }

    public void SpawnItemFromDatabaseCMD(int index, Vector3 place)
    {
        _itemsManager.SpawnItem(_itemDatabase.EntryList[index], place, Quaternion.identity);
    }
}
