using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<Vector3> AllPositions => _allPositions;
    public TrackPath[] Path => _path;
    public bool Looped => _looped;
    [SerializeField, FoldoutGroup("Path")] private TrackPath[] _path;
    [SerializeField, FoldoutGroup("Path")] private bool _looped;
    [SerializeField, FoldoutGroup("Gizmos")] private bool _drawGizmos;
    [SerializeField, FoldoutGroup("Gizmos")] private Color _gizmosColor;
    [SerializeField, FoldoutGroup("Gizmos")] private float _gizmosRadius = 0.1f;

    [SerializeField, FoldoutGroup("Positions")] private List<Vector3> _allPositions = new();

    [Button("SetTrack")]
    public void SetTrack()
    {
        _allPositions.Clear();

        for (int i = 0; i < _path.Length - 1; i++)
        {
            for (var j = 0; j < _path[i].MaxPoints; j++)
            {
                var newPosition = CubicCurve(_path[i].Target.position, _path[i].Target.position + _path[i].Offset, _path[i].Target.position + _path[i].Offset,
                    _path[i + 1].Target.position, (float)j / _path[i].MaxPoints);
                _allPositions.Add(newPosition);
            }
        }

        Debug.Log($"{gameObject.name} set new track");
    }

    private Vector3 CubicCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        return (((-start + 3 * (control1 - control2) + end) * t + (3 * (start + control2) - 6 * control1)) * t +
                3 * (control1 - start)) * t + start;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;

        if (!_drawGizmos)
            return;

        for (int i = 0; i < _path.Length - 1; i++)
        {
            for (var j = 0; j <= _path[i].MaxPoints; j++)
            {
                var newPosition = CubicCurve(_path[i].Target.position, _path[i].Target.position + _path[i].Offset, _path[i].Target.position + _path[i].Offset,
                    _path[i + 1].Target.position, (float)j / _path[i].MaxPoints);
                Gizmos.DrawSphere(newPosition, _gizmosRadius);
            }
        }
    }
}

[Serializable]
public class TrackPath
{
    public Transform Target;
    public Vector3 Offset;
    public int MaxPoints;
}
