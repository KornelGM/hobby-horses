#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PadlocksDatabase", menuName = "ScriptableObjects/Padlock/PadlocksDatabase")]
[Serializable]
public class PadlocksDatabase : DatabaseElement<Padlock>
{
#if UNITY_EDITOR
    [InfoBox("This controller only allows you to modify lock status, list presents current Entry List state")]
    [CustomValueDrawer(nameof(MyCustomPadlockDrawer))]
#endif
    public List<Padlock> ControllerInterface = new();
    public override List<string> GetPath()
    {
        return new List<string>();
    }

#if UNITY_EDITOR
    public override void RefreshDictionary()
    {
        base.RefreshDictionary();
        ControllerInterface = new(EntryList);
    }
    public Padlock MyCustomPadlockDrawer(Padlock value, GUIContent label, Func<GUIContent, bool> callNextDrawer)
    {
        SirenixEditorGUI.BeginBox();

        bool locked = Application.isPlaying? value.Locked: value.LockedDefault;
        GUILayout.BeginHorizontal();
        GUILayout.Label(value.name, GUILayout.Width(150));

        if (locked)
        {
            GUI.color = Color.red;
            if (GUILayout.Button(Application.isPlaying?"Locked" : "Default Locked", GUILayout.Width(150)))
            {
                if (Application.isPlaying) value.Unlock();
                else value.UnlockDefault();
            }
        }
        else
        {
            GUI.color = Color.green;
            if (GUILayout.Button(Application.isPlaying ? "Unlocked" : "Default Unlocked", GUILayout.Width(150)))
            {
                if (Application.isPlaying) value.Lock();
                else value.LockDefault();
            }
        }

        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        SirenixEditorGUI.EndBox();
        return value;
    }
    #endif
}
