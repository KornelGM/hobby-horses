using I2.Loc;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "ScriptableObjects/Items/Item Data")]
public class ItemData : DatabaseElement<ServiceLocator>
{
	public string Id;
    [Space(10)]
    [SerializeField, FoldoutGroup("Inventory Data")] public LocalizedString Name;
    [SerializeField, FoldoutGroup("Inventory Data")] public UIMipmap Icon;
    [SerializeField, FoldoutGroup("Inventory Data"),Range(1, 100)] public int Amount = 1;
    [SerializeField, FoldoutGroup("Inventory Data")] public bool AbleToChangeSlot = true;

    [SerializeField, FoldoutGroup("Item Prefab")] public ServiceLocator Prefab;

    [SerializeField, FoldoutGroup("Settings")] public ItemType Type;
    [LabelWidth(185)]
    [SerializeField, FoldoutGroup("Settings")] public bool CanBeCarryingInBox;
    [LabelWidth(185)]
    [SerializeField, FoldoutGroup("Settings")] public bool VisibleOnWarehouseList;    
    [LabelWidth(185)]
    [SerializeField, FoldoutGroup("Settings"), ShowIf(nameof(VisibleOnWarehouseList))] public bool NougatCantBring;


    [InfoBox("Warning! An item available for purchase in the store MUST have a prefab assigned!", 
        InfoMessageType.Warning, visibleIfMemberName: "_prefabIsNull")]

    [SerializeField, FoldoutGroup("Shop Info"), DisableIf("_prefabIsNull")] public bool AbleToBuy;
    [SerializeField, FoldoutGroup("Shop Info"), ShowIf("AbleToBuy")] public int BasePrice;
    [SerializeField, FoldoutGroup("Shop Info"), ShowIf("AbleToBuy")] public ItemShopType ShopType;
    [SerializeField, FoldoutGroup("Shop Info"), ShowIf("AbleToBuy")] public LocalizedString ShopDescription;
    [SerializeField, FoldoutGroup("Shop Info"), ShowIf("AbleToBuy"), Range(0, 5)] public int ReputationLevel;


    private const string _termName = "Items";
    private const string _termDescription = "Item Descriptions";

    [Button("Try get item name"), FoldoutGroup("Buttons")]
    private void TryGetItemName()
    {
        string nameTerm = $"{_termName}/{Id}";

        Name.mTerm = nameTerm;
    }

    [Button("Try get item description"), FoldoutGroup("Buttons"), ShowIf("AbleToBuy")]
    private void TryGetItemDescription()
    {
        string descriptionTerm = $"{_termDescription}/{Id}";

        ShopDescription.mTerm = descriptionTerm;
    }


    [field: SerializeField] public Vector3 Offset { get; set; } = new Vector3(0.3f, -0.4f, 0.6f);
    [field: SerializeField] public Vector3 Rotation { get; set; } = new Vector3(0.87f, 37, 339);
    [field: SerializeField, Range(0, 1)] public float CenterZ { get; set; } = 0;
    public bool AttachedToCamera { get; set; } = true;
    public override List<string> GetPath() => new();

    private bool _prefabIsNull { get { return Prefab == null; } }
    public string GetItemName()
    {
        if (string.IsNullOrEmpty(Name)) return Id;

        return Name;
    }
}