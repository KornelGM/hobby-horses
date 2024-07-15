using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLinksBoundsSettings : MonoBehaviour
{
    public static readonly Color defaultGizmoColor = new(0f, 1f, 0f, 0.5f);

    public Color gizmoColor = new(0f, 1f, 0f, 0.5f);
    public bool alwaysShowBoundingBoxes = false;

}
