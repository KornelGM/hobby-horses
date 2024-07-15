using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelFollowTransform : MonoBehaviour
{
    private Transform _transformToFollow;
    private Vector3 _offset = Vector3.up;
    private bool _follow = false;
    
    void LateUpdate()
    {
        UpdatePosition();
    }

    public void Init(Transform transformToFollow, Vector3 offset)
    {
        _transformToFollow = transformToFollow;
        _offset = offset;
        _follow = true;
    }

    private void UpdatePosition()
    {
        if (!_follow) return;
        transform.position = _transformToFollow.position + _offset;
    }
    
}
