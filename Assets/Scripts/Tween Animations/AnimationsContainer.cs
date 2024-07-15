using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetsForAnimation
{
    public Transform target;
    public ItemData item;
}

public class AnimationsContainer : MonoBehaviour
{
    public List<TargetsForAnimation> TargetForAnimation => _targetsForAnimation;

    [TableList(ShowIndexLabels = true)]
    [SerializeField] private List<TargetsForAnimation> _targetsForAnimation;
}
