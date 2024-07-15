using I2.Loc;
using Rewired;
using UnityEngine;


[CreateAssetMenu(fileName = "Key Binding Setting", menuName = "ScriptableObjects/Settings/KeyBinding")]
public class KeyBindingSettingAsset : ScriptableObject
{
    [field: SerializeField] public LocalizedString DisplayedName { get; private set; }
    [field: SerializeField] public string FallbackDisplayedName { get; private set; }
    [field: SerializeField] public int ActionId { get; private set; }
    [field: SerializeField] public BindingControllerType ControllerType { get; private set; } = BindingControllerType.KeyboardAndMouse;

    [field: SerializeField] public bool IsAxis { get; private set; } = false;
    [field: SerializeField] public AxisRange AxisRange { get; private set; } = AxisRange.Positive;
}