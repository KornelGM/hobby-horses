using System;
using UnityEngine;
using Pathfinding;
using System.Collections;

public class PathfindingService : MonoBehaviour, IUpdateable, IStartable, IPathfindingService, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public event Action OnDestinationReached;
    public event Action OnDestinationNotReachable;
    public event Action OnStoppableEnabled;
    public event Action OnStoppableDisabled;

    public bool CanJump
    {
        get => _canJump;
        set
        {
            if (CanJump == value)
            {
                return;
            }

            _canJump = value;
            _seeker.traversableTags ^= 1 << _pathfindingServiceSettings.JumpTag;
        }
    }

    public bool IsSprint
    {
        get => _virtualController.IsSprint;
        set
        {
            _virtualController.IsSprint = value;
            _virtualController.IsWalk = false;
            _richAI.maxSpeed = _stateMachine.MovementSettings.MovementSpeed * (value ? _stateMachine.MovementSettings.SprintSpeedModification : _stateMachine.MovementSettings.NormalAnimatorMovementSpeed);
        }
    }

    public bool IsWalk
    {
        get => _virtualController.IsWalk;
        set
        {
            _virtualController.IsWalk = value;
            _virtualController.IsSprint = false;
            _richAI.maxSpeed = _stateMachine.MovementSettings.MovementSpeed * (value ? _stateMachine.MovementSettings.WalkAnimatorMovementSpeed : _stateMachine.MovementSettings.NormalAnimatorMovementSpeed);
        }
    }
    private AgentMoveType _moveType = AgentMoveType.Jog;
    public AgentMoveType MoveType
    {
        get => _moveType;
        set
        {
            switch (value)
            {
                case AgentMoveType.Walk:
                    _virtualController.IsWalk = true;
                    _virtualController.IsSprint = false;
                    _richAI.maxSpeed = _stateMachine.MovementSettings.MovementSpeed * _stateMachine.MovementSettings.WalkAnimatorMovementSpeed;
                    break;
                case AgentMoveType.Jog:
                    _virtualController.IsWalk = false;
                    _virtualController.IsSprint = false;
                    _richAI.maxSpeed = _stateMachine.MovementSettings.MovementSpeed * _stateMachine.MovementSettings.NormalAnimatorMovementSpeed;
                    break;
                case AgentMoveType.Sprint:
                    _virtualController.IsWalk = false;
                    _virtualController.IsSprint = true;
                    _richAI.maxSpeed = _stateMachine.MovementSettings.MovementSpeed * _stateMachine.MovementSettings.SprintAnimatorMomenentSpeed;
                    break;
            }
            _moveType = value;
        }
    }

    public bool Enabled => true;

    [SerializeField] PathfindingServiceSettings _pathfindingServiceSettings;
    [SerializeField] PathFindingTraversableSettings _traversableSettings;
    [SerializeField] bool _canJump;
    [SerializeField] float slowdownDistance = 5f;
    [SerializeField] bool slowdownMovementWhenTurning = false;
    [SerializeField] GameObject endBlockadePrefab;

    private GameObject _activeBlockade;
    private MoveableCharacterStateMachine _stateMachine;
    private IVirtualController _virtualController;
    private RichAI _richAI;
    private Seeker _seeker;
    private Path _previousPath = null;
    private Vector3 _overridenMovement;
    private float _destinationRotation;
    private float _defaultEndReachedDistance;
    private float _targetEndReachedDistance;
    private bool _useDestinationRotation = false;
    private bool _overrideMovement = false;
    private bool _scheduledDisabling = false;
    private bool _sentReachedDestination = true;
    private bool _reachedDestination = true;
    private bool _destinationSet = false;
    private bool _setSprinting = false;

    private const float OffmeshTimeBuffer = 1.05f;

    public void CustomStart()
    {
        _seeker = GetComponent<Seeker>();
        _richAI = GetComponent<RichAI>();
        _richAI.onTraverseOffMeshLink += TraverseOffMeshLink;
        _defaultEndReachedDistance = _richAI.endReachedDistance;
        MyServiceLocator.TryGetServiceLocatorComponent(out _virtualController);
        MyServiceLocator.TryGetServiceLocatorComponent(out _stateMachine);
        _pathfindingServiceSettings.IsNotNull(this);

        _traversableSettings.IsNotNull(this);
        _seeker.traversableTags = _traversableSettings.TagMask;
    }

    void OnDisable()
    {
        OnDestinationNotReachable?.Invoke();
    }

    public void OnDestroy()
    {
        if (_richAI != null)
        {
            _richAI.onTraverseOffMeshLink -= TraverseOffMeshLink;
        }
    }

    public void CustomUpdate()
    {
        CheckAIMovement();
        CheckIfShouldDisable();
    }

    public void EnablePathfinding()
    {
        _richAI.canMove = true;
        _scheduledDisabling = false;
    }

    public void DisablePathfinding()
    {
        if (!Stop())
        {
            _scheduledDisabling = true;
        }
        else
        {
            _richAI.canMove = false;
            _virtualController.Movement = Vector3.zero;
        }
    }

    public bool IsPathfindingEnabled()
    {
        return _richAI.canMove;
    }

    public bool CanStop()
    {
        return !_overrideMovement;
    }

    public bool Stop()
    {
        if (!_overrideMovement)
        {
            _richAI.destination = transform.position;
            StopAllCoroutines();
            return true;
        }

        return false;
    }

    public bool CanReachDestinationArea(Vector3 destination)
    {
        if (!AstarPath.active)
        {
            return false;
        }

        NNConstraint constraint = NNConstraint.Default;
        constraint.tags = _seeker.traversableTags;

        GraphNode start = AstarPath.active.GetNearest(transform.position, constraint).node;
        GraphNode end = AstarPath.active.GetNearest(destination, constraint).node;

        return PathUtilities.IsPathPossible(start, end);
    }

    public bool CanReachDestination(Vector3 destination)
    {
        if (!AstarPath.active)
        {
            return false;
        }

        NNConstraint constraint = NNConstraint.Default;
        constraint.tags = _seeker.traversableTags;
        NNInfo start = AstarPath.active.GetNearest(transform.position, constraint);
        NNInfo end = AstarPath.active.GetNearest(destination, constraint);

        Vector3 destination2D = new(destination.x, 0f, destination.z);
        Vector3 end2D = new(end.position.x, 0f, end.position.z);

        if ((destination2D - end2D).sqrMagnitude < _defaultEndReachedDistance * _defaultEndReachedDistance)
        {
            return PathUtilities.IsPathPossible(start.node, end.node);
        }

        return false;
    }

    public void SetDestination(Vector3 destination)
    {
        if (!_richAI.canMove)
        {
            return;
        }

        SetDestination(destination, _defaultEndReachedDistance);
    }

    public void SetDestination(Vector3 destination, float endReachedDistance)
    {
        if (!_richAI.canMove)
        {
            return;
        }

        _previousPath = _seeker.GetCurrentPath();
        _richAI.endReachedDistance = endReachedDistance;
        _richAI.destination = destination;
        _sentReachedDestination = false;
        _reachedDestination = false;
        _destinationSet = true;
        _setSprinting = MoveType == AgentMoveType.Sprint;
    }

    public void SetDestinationWithRotation(Vector3 destination, Vector3 rotation, bool useBlockades = true)
    {
        SetDestinationWithRotation(destination, rotation, _defaultEndReachedDistance * 2f, useBlockades);
    }

    // Note: When setting destination with rotation for horse and other slow-turning agents
    // it's best to make the end reached distance bigger, like 50-100% bigger
    // it will still try to get as close as possible, but it's easier if it's bigger
    public void SetDestinationWithRotation(Vector3 destination, Vector3 rotation, float endReachedDistance, bool useBlockades = true)
    {
        rotation.x = 0f;
        rotation.z = 0f;

        if (useBlockades && !endBlockadePrefab)
        {
            Debug.LogWarning("End blockade prefab not set up, the resulting path will use just in-place rotation");
        }

        if (useBlockades)
        {
            if (_activeBlockade)
            {
                _activeBlockade.transform.SetPositionAndRotation(destination, Quaternion.Euler(rotation));
            }
            else
            {
                _activeBlockade = Instantiate(endBlockadePrefab, destination, Quaternion.Euler(rotation));
            }

        }

        _destinationRotation = Mathf.DeltaAngle(0f, rotation.y);
        _useDestinationRotation = true;
        _targetEndReachedDistance = endReachedDistance;

        SetDestination(destination, endReachedDistance / 2f);
    }

    public void ClearOnDestinationReached() => OnDestinationReached = null;
    public void ClearOnDestinationNotReachable() => OnDestinationNotReachable = null;

    private void CheckIfShouldDisable()
    {
        if (_scheduledDisabling && CanStop())
        {
            _scheduledDisabling = false;
            _richAI.canMove = false;
        }
    }

    private void CheckAIMovement()
    {
        if (!_richAI.canMove || _previousPath == _seeker.GetCurrentPath())
        {
            return;
        }

        float angle = Mathf.DeltaAngle(_destinationRotation, transform.rotation.eulerAngles.y);

        if (_richAI.canMove && !_richAI.reachedEndOfPath)
        {
            _destinationSet = false;
            _reachedDestination = false;

            if (_overrideMovement)
            {
                _virtualController.Movement = _overridenMovement;
                return;
            }

            if (MoveType == AgentMoveType.Sprint & _richAI.approachingPartEndpoint && _richAI.distanceToSteeringTarget <= slowdownDistance)
            {
                MoveType = AgentMoveType.Jog;
            }

            Vector3 movement = _richAI.desiredVelocity;
            movement.y = 0f;
            movement.Normalize();

            if (slowdownMovementWhenTurning)
            {
                float signAngle = Vector3.SignedAngle(transform.forward, movement, transform.up);
                float unsignAngle = Mathf.Abs(signAngle);
                signAngle = Mathf.Sign(signAngle) * Mathf.Clamp(unsignAngle, 0f, 179.5f - unsignAngle);
                float angleToTurnAngle = Mathf.Clamp01(Mathf.Abs(signAngle) / _stateMachine.MovementSettings.TurnAngle);
                movement = Quaternion.AngleAxis(signAngle * angleToTurnAngle * angleToTurnAngle, transform.up) * movement;

                float movementChange = 1.5f - Mathf.Clamp(unsignAngle / 90f, 0.5f, 1f);
                movementChange *= Mathf.Clamp01(_richAI.distanceToSteeringTarget / slowdownDistance);

                if (_setSprinting)
                {
                    if (movementChange < (1f / _stateMachine.MovementSettings.SprintSpeedModification))
                    {
                        movementChange *= _stateMachine.MovementSettings.SprintSpeedModification;
                        _virtualController.IsSprint = false;
                    }
                    else
                    {
                        _virtualController.IsSprint = true;
                    }
                }

                movement *= movementChange;
            }

            _virtualController.Movement = movement;
        }
        else if (_richAI.canMove && _richAI.reachedEndOfPath && _useDestinationRotation && slowdownMovementWhenTurning && Mathf.Abs(angle) > 15f)
        {
            _richAI.endReachedDistance = _targetEndReachedDistance;
            MoveType = AgentMoveType.Jog;
            _virtualController.Movement =
                Quaternion.AngleAxis(-Mathf.Sign(angle) * Mathf.Clamp(Mathf.Abs(angle) * 1.5f, 0f, _stateMachine.MovementSettings.TurnAngle), transform.up) * transform.forward * 0.1f;
        }
        else if (((_richAI.reachedEndOfPath && !_richAI.pathPending && _richAI.hasPath) || (!_destinationSet && !_reachedDestination)) && (!_useDestinationRotation || Mathf.Abs(angle) <= 15f))
        {
            if (!_sentReachedDestination)
            {
                OnDestinationReached?.Invoke();
                _sentReachedDestination = true;
            }

            if (_activeBlockade)
            {
                Destroy(_activeBlockade);
            }

            _reachedDestination = true;
            _destinationSet = false;
            _useDestinationRotation = false;
            _virtualController.Movement = Vector3.zero;
            _stateMachine.Velocity = Vector3.zero;
        }
    }

    public void SetMovingDestination(Transform target, float endReachedDistance = -1f, float repathTime = 0.1f)
    {
        var cor = StartCoroutine(SetMovingDestinationInternal(target, endReachedDistance < 0f ? _defaultEndReachedDistance : endReachedDistance, repathTime));
        OnDestinationReached += () => StopCoroutine(cor);
    }

    private IEnumerator SetMovingDestinationInternal(Transform target, float endReachedDistance, float repathTime)
    {
        if (!CanReachDestinationArea(target.position))
        {
            OnDestinationNotReachable?.Invoke();
            yield break;
        }

        WaitForSeconds wait = new(repathTime);
        float endReachedDistanceDiv2Sqr = (endReachedDistance / 2f) * (endReachedDistance / 2f);

        SetDestination(target.position, endReachedDistance);
        while (true)
        {
            yield return wait;

            if (!CanReachDestinationArea(target.position))
            {
                OnDestinationNotReachable?.Invoke();
                break;
            }

            Vector3 destination = new(_richAI.destination.x, 0f, _richAI.destination.z);
            Vector3 position = new(target.position.x, 0f, target.position.z);

            if ((destination - position).sqrMagnitude > endReachedDistanceDiv2Sqr)
                SetDestination(target.position, endReachedDistance);
        }
    }

    private IEnumerator TraverseOffMeshLink(RichSpecial link)
    {
        _overridenMovement = link.second.position - transform.position;
        _overridenMovement.y = 0f;
        // Add some time in case the agent just started moving and is not at full speed
        float time = _overridenMovement.magnitude / _richAI.maxSpeed * OffmeshTimeBuffer;
        _overridenMovement.Normalize();
        float wallForce = _richAI.wallForce;
        _richAI.wallForce = 0f;
        OnStoppableDisabled?.Invoke();
        _overrideMovement = true;

        yield return new WaitForSeconds(time * 0.2f);

        if (_canJump && link.second.position.y > link.first.position.y)
        {
            _virtualController.OnJumpPerformed?.Invoke();
        }

        yield return new WaitForSeconds(time * 0.8f);
        _overrideMovement = false;
        OnStoppableEnabled?.Invoke();
        _richAI.wallForce = wallForce;
    }
}