#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System;

public static class OdinUtilities
{
    public static IEnumerable<ValueDropdownItem> GetAllScriptables(Type type)
    {
        string className = type.ToString();
        return AssetDatabase.FindAssets($"t:{className}").Select(x => AssetDatabase.GUIDToAssetPath(x))
        .Select(x => new ValueDropdownItem(x, AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));
    }
}

#endif