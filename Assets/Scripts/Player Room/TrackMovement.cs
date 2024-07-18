using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TrackMovement : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Settings")] private float _speed;

    public Action OnStartPath;
    public Action OnEndPath;

    private int _currentTargetPoint;
    private Track _currentTrack;
    private bool _isMoving;
    private bool _revert;

    private void Update()
    {
        if (_isMoving)
        {
            GoToPoint();
        }
    }

    private void GoToPoint()
    {
        if(transform.position == _currentTrack.AllPositions[_currentTargetPoint])
        {
            SetNewTargetPoint();
        }
        if(_isMoving)
            transform.position = Vector3.MoveTowards(transform.position, _currentTrack.AllPositions[_currentTargetPoint], _speed * Time.deltaTime);

    }

    private void SetNewTargetPoint()
    {
        if (_revert)
        {
            _currentTargetPoint--;
            if (_currentTargetPoint < 0)
            {
                if (_currentTrack.Looped)
                {
                    Move(_currentTrack, _revert);
                }
                else
                {
                    _isMoving= false;
                }
                OnEndPath?.Invoke();
            }
        }
        else
        {
            _currentTargetPoint++;
            if (_currentTargetPoint >= _currentTrack.AllPositions.Count)
            {
                if (_currentTrack.Looped)
                {
                    Move(_currentTrack, _revert);
                }
                else
                {
                    _isMoving = false;
                }
                OnEndPath?.Invoke();
            }
        }
    }

    public void Move(Track track, bool revert = false)
    {
        _currentTrack = track;
        _revert = revert;

        _currentTargetPoint = _revert ? _currentTrack.AllPositions.Count - 1 : 0;

        _isMoving = true;
        OnStartPath?.Invoke();
    }
}
