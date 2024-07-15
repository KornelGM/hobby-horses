using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using System.Linq;

public enum PathFindingNodeTags
{
    BasicGround,
    ClosedDoor,
    ClosedPetDoor,
    OpenedPetDoor,
    Jump,
}

[CreateAssetMenu(fileName = "Pathfinding Tag Settings", menuName = "ScriptableObjects/Pathfinding/PathfindingTravesableTagsSettings")]
public class PathFindingTraversableSettings : SerializedScriptableObject
{
    [ValueDropdown("GetAllTags", IsUniqueList = true), SerializeField]
    private readonly List<PathFindingNodeTags> traversableTags =
                                new List<PathFindingNodeTags>(Enum.GetValues(typeof(PathFindingNodeTags)).Cast<PathFindingNodeTags>());

    [ReadOnly] public int TagMask;

    private static IEnumerable<ValueDropdownItem> GetAllTags()
    {
        var enumArray = Enum.GetValues(typeof(PathFindingNodeTags)).Cast<PathFindingNodeTags>();

        return enumArray.Select(x => new ValueDropdownItem(x.ToString(), x));
    }

    private void OnValidate()
    {
        TagMask = ConvertTagsToMask();
        traversableTags.Sort();
    }

    private int ConvertTagsToMask()
    {
        int mask = 0;

        foreach (var tag in traversableTags)
        {
            mask += 1 << ((int)tag);
        }
        return mask;
    }
}


