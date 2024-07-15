using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AgentTargetSetter : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private Transform _pointsOfInterest;

    private PathfindingService _pathfinding;
    private WaitForSeconds _wait = new(1f);

    public void CustomStart()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out _pathfinding);
        _pointsOfInterest.IsNotNull(this);

        _pathfinding.OnDestinationReached += OnDestinationReached;
        StartCoroutine(SetNewDestination());
    }
    private void OnDestroy()
    {
        _pathfinding.OnDestinationReached -= OnDestinationReached;
    }

    private IEnumerator SetNewDestination()
    {
        yield return _wait;
        Vector3 destination = _pointsOfInterest.GetChild(Random.Range(0, _pointsOfInterest.childCount)).transform.position;
        if (!_pathfinding.CanReachDestinationArea(destination))
        {
            transform.position = AstarPath.active.GetNearest(transform.position).position;
        }

        _pathfinding.MoveType = AgentMoveType.Walk;
        _pathfinding.SetDestination(destination);
    }

    private void OnDestinationReached()
    {
        StartCoroutine(SetNewDestination());
    }
}
