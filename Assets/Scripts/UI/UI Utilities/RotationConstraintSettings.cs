using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RotationConstraintSettings : MonoBehaviour
{
    [Header("Choose one")]
    [SerializeField] private Transform _constraintTransform=null;
    [SerializeField] private TransformVariable _constraintTransformVariable=null;
    [SerializeField] private bool _constrantToCamera=false;

    private RotationConstraint _rotationConstraint;

    void Awake()
    {
        _rotationConstraint = GetComponent<RotationConstraint>();
        _rotationConstraint.IsNotNull(this, nameof(_rotationConstraint));
    }

    void Start()
    {
        RestrainRotation();
    }

    private void RestrainRotation()
    {
        _rotationConstraint = GetComponent<RotationConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();

        if (_constraintTransform!=null) constraintSource.sourceTransform = _constraintTransform;
        if (_constraintTransformVariable != null) constraintSource.sourceTransform = _constraintTransformVariable.Value;
        if (_constrantToCamera) constraintSource.sourceTransform = Camera.main.transform;

        constraintSource.weight = 1;
        _rotationConstraint.AddSource(constraintSource);
    }
}
