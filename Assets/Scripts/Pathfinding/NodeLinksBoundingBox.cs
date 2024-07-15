using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLinksBoundingBox : MonoBehaviour
{

    [InfoBox("You can rotate this object")]
    [InfoBox("Z value is only used for showing the box, it will not be used in node generator", "_zChanged")]
    [Min(0f)]
    public Vector3 size = new(1f, 1f, 1f);
    private bool _zChanged = false;

    private void OnValidate()
    {
        _zChanged = size.z != 1f;
    }

    private void OnDrawGizmosSelected()
    {
        NodeLinksBoundsSettings settings = FindObjectOfType<NodeLinksBoundsSettings>();

        if (!settings || !settings.alwaysShowBoundingBoxes)
        {
            ShowGizmos(settings);
        }
    }

    private void OnDrawGizmos()
    {
        NodeLinksBoundsSettings settings = FindObjectOfType<NodeLinksBoundsSettings>();

        if (settings && settings.alwaysShowBoundingBoxes)
        {
            ShowGizmos(settings);
        }
    }

    private void ShowGizmos(NodeLinksBoundsSettings settings)
    {
        Gizmos.color = settings ? settings.gizmoColor : NodeLinksBoundsSettings.defaultGizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(
            Vector3.zero,
            new Vector3(
                size.x / Mathf.Abs(transform.lossyScale.x),
                size.y / Mathf.Abs(transform.lossyScale.y),
                size.z / Mathf.Abs(transform.lossyScale.z)
            )
        );
    }
}
