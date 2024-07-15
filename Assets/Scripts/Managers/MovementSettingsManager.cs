using UnityEngine;

public class MovementSettingsManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private MovementSettings _movementSettings;

    public float TurnSmoothTime => _movementSettings.TurnSmoothTime;
    public float MovementSpeed => _movementSettings.MovementSpeed;
    public float SprintSpeedModification => _movementSettings.SprintSpeedModification;
    public float JumpForce => _movementSettings.JumpForce;
    public float GroundedOffset => _movementSettings.GroundedOffset;
    public float JumpAdditionalOffset => _movementSettings.JumpAdditionalOffset;
    public float GroundedRadius => _movementSettings.GroundedRadius;
    public LayerMask GroundLayers => _movementSettings.GroundLayers;
    public float FallTimeout => _movementSettings.FallTimeout;

    public float NormalAnimatorMovementSpeed => _movementSettings.NormalAnimatorMovementSpeed;
    public float SprintAnimatorMomenentSpeed => _movementSettings.SprintAnimatorMomenentSpeed;
    public float WalkAnimatorMovementSpeed => _movementSettings.WalkAnimatorMovementSpeed;
    public float RotationSpeed => _movementSettings.RotationSpeed;
    public float GamepadRotationSpeedMultiplier => _movementSettings.GamepadRotationSpeedMultiplier;
    public float TurnAngle => _movementSettings.TurnAngle;
    public float HorizontalMoveSmoother => _movementSettings.HorizontalMoveSmoother;
    public float VerticalMoveSmoother => _movementSettings.VerticalMoveSmoother;
}
