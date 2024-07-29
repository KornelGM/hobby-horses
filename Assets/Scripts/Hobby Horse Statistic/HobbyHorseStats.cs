using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HH Stats", menuName = "ScriptableObjects/Hobby Horse Stats/new HH Stats")]
public class HobbyHorseStats : ScriptableObject
{
    public float MaxSpeed => _maxSpeed;
    public float Accelerate => _accelerate;
    public float BrakeForce => _brakeForce;
    public float MaxRotateSpeed => _maxRotateSpeed;
    public float RotateAccelerate => _rotateAccelerate;

    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _maxSpeed;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _maxRotateSpeed;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _brakeForce;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _accelerate;
    [SerializeField, FoldoutGroup("Hobby Horse Settings")] private float _rotateAccelerate;
}
