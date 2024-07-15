using System;
using UnityEngine;

public enum AgentMoveType
{
    Walk,
    Jog,
    Sprint
}

public interface IPathfindingService
{
    public event Action OnDestinationReached;
    public event Action OnDestinationNotReachable;
    public event Action OnStoppableEnabled;
    public event Action OnStoppableDisabled;

    public bool CanJump { get; set; }
    public bool IsSprint { get; set; }
    public bool IsWalk { get; set; }
    public AgentMoveType MoveType {get; set;}
    public void EnablePathfinding();
    public void DisablePathfinding();
    public bool IsPathfindingEnabled();
    public bool Stop();
    public bool CanStop();
    public bool CanReachDestinationArea(Vector3 destination);
    public bool CanReachDestination(Vector3 destination);
    public void SetDestination(Vector3 destination);
    public void SetDestination(Vector3 destination, float endReachedDistance);
    public void SetDestinationWithRotation(Vector3 destination, Vector3 rotation, bool useBlockades);
    public void SetDestinationWithRotation(Vector3 destination, Vector3 rotation, float endReachedDistance, bool useBlockades);
    public void ClearOnDestinationReached();
    public void ClearOnDestinationNotReachable();
    public void SetMovingDestination(Transform target, float endReachedDistance = -1f, float repathTime = 0.1f);
}
