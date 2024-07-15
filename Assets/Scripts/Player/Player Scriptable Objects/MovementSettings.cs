using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Movement Settings", menuName = "ScriptableObjects/Player/PlayerMovementSettings")]
public class MovementSettings : ScriptableObject
{
    [field: SerializeField] public float TurnSmoothTime { get; private set; } = 0.1f;
    [field: SerializeField] public float MovementSpeed { get; private set; } = 5f;
    [field: SerializeField] public float SprintSpeedModification { get; private set; } = 2f;
    [field: SerializeField] public float JumpForce { get; private set; } = 5f;
    [field: SerializeField] public float GroundedOffset { get; private set; } = -0.14f;
    [field: SerializeField] public float JumpAdditionalOffset { get; private set; } = 0.1f;
    [field: SerializeField] public float GroundedRadius { get; private set; } = 0.28f;
    [field: SerializeField] public LayerMask GroundLayers { get; private set; }
    [field: SerializeField] public float FallTimeout { get; private set; } = 0.15f;

    [field: SerializeField] public float NormalAnimatorMovementSpeed { get; private set; } = 2f;
    [field: SerializeField] public float SprintAnimatorMomenentSpeed { get; private set; } = 3f;
    [field: SerializeField] public float WalkAnimatorMovementSpeed { get; private set; } = 1f;
    [field: SerializeField] public float RotationSpeed { get; set; } = 12;
    [field: SerializeField] public float GamepadRotationSpeedMultiplier = 4f;
    [field: Range(0f, 360f)]
    [field: SerializeField]
    [field: Tooltip("Specifies  how much agent can bend when turning")]
    public float TurnAngle { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Determines how smoothly the agent responds to input in horizontal axis")]
    public float HorizontalMoveSmoother { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Determines how smoothly the agent responds to input in vertical axis")]
    public float VerticalMoveSmoother { get; private set; }

}
