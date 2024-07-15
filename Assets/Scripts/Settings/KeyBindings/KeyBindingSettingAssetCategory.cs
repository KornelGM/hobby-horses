using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "Key Binding Setting Category", menuName = "ScriptableObjects/Settings/KeyBinding Category")]
public class KeyBindingSettingAssetCategory : ScriptableObject
{
    [field: SerializeField] public LocalizedString DisplayedName { get; private set; }
    [field: SerializeField] public int CategoryId { get; private set; }
    [field: SerializeField] public KeyBindingSettingAsset[] KeyBindingSettingsAssets { get; private set; }
}