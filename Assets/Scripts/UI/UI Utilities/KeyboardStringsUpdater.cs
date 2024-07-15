using LMirman.RewiredGlyphs;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardStringsUpdater : MonoBehaviour
{
    static public Dictionary<KeyCode, string> Keys = new();

    [Button("Print Keys")]
    private void PrintKeys()
    {
        foreach (var key in Keys)
            Debug.Log(key.Key + " " + key.Value);
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += DeviceChanged;

        UpdateDictionary();
    }

    private void DeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (device is Keyboard && UpdateDictionary())
            InputGlyphs.InvokeRebuild(true);
    }

    public bool UpdateDictionary()
    {
        var keyboard = InputSystem.GetDevice<Keyboard>();

        if (keyboard == null)
            return false;

        bool keysChanged = false;

        foreach (var keyControl in keyboard.allKeys)
        {
            KeyCode keyCode = InputUtilities.KeyToKeyCode(keyControl.keyCode);

            if (keyCode == KeyCode.None)
                continue;

            if (Keys.TryGetValue(keyCode, out string value))
            {
                if (value == keyControl.displayName)
                    continue;

                Keys[keyCode] = keyControl.displayName;
            }
            else
                Keys.TryAdd(keyCode, keyControl.displayName);

            keysChanged = true;
        }

        return keysChanged;
    }
}
