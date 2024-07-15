#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class ShopBalancer : OdinMenuEditorWindow
{
    public ShopEditorType ShopEditorType = new();
    private ItemShopType _currentShopType = ItemShopType.All;

    [MenuItem("Tools/Dream Parable/Shop Balancer")]
    private static void OpenEditor() => GetWindow<ShopBalancer>();

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        tree.Add("Select Shop Type", ShopEditorType);
        if (ShopEditorType.ShopType == ItemShopType.All)
        {
            foreach (var item in GetShoItems())
            {
                tree.Add(item.name, item);
            }
        }
        else
        {
            foreach (var item in GetShoItemsOfType(ShopEditorType.ShopType))
            {
                tree.Add(item.name, item);
            }
        }

        _currentShopType = ShopEditorType.ShopType;
        return tree;
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        if (_currentShopType == ShopEditorType.ShopType)
            return;

        this.ForceMenuTreeRebuild();

    }

    public static List<ItemData> GetShoItemsOfType(ItemShopType itemShopType)
    {
        return GetShoItems().Where(item => item.ShopType == itemShopType).ToList();
    }

    public static List<ItemData> GetShoItems()
    {
        return FindAllItemData().Where(item => item.AbleToBuy == true).ToList();
    }

    public static List<ItemData> FindAllItemData()
    {
        List<ItemData> results = new();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(ItemData).Name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData asset = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (asset != null)
            {
                results.Add(asset);
            }
        }

        return results;
    }
}

[Serializable]
public class ShopEditorType
{
    public ItemShopType ShopType = ItemShopType.All;

    [Button("Get Debug Items List")]
    private void GetItemsList()
    {
        string list = "";

        foreach (var item in ShopBalancer.GetShoItems())
        {
            list += $"{item.Id} \n";
        }

        UnityEngine.Debug.Log(list);
    }
}

#endif
