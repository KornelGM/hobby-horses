using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public Transform[] Path => _path;
    public bool Looped => _looped;
    [SerializeField, FoldoutGroup("Path")] private Transform[] _path;
    [SerializeField, FoldoutGroup("Path")] private bool _looped;
    [SerializeField, FoldoutGroup("Gizmos")] private bool _drawGizmos;
    [SerializeField, FoldoutGroup("Gizmos")] private Color _gizmosColor;
    [SerializeField, FoldoutGroup("Gizmos")] private float _gizmosRadius = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;

        if (!_drawGizmos)
            return;

        for (int i = 0; i < _path.Length; i++)
        {
            Gizmos.DrawSphere(_path[i].position, _gizmosRadius);
            if (i + 1 == _path.Length)
                continue;

            Gizmos.DrawLine(_path[i].position, _path[i + 1].position);
        }
    }
}
