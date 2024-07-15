using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITooltipPoint 
{
    public void UpdatePosition();
    public Vector3 Point { get; }
}
