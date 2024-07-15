using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transform Variable", menuName = "ScriptableObjects/Variables/TransformVariable")]
public class TransformVariable : ScriptableVariable<Transform>
{
    public virtual void SetValue(TransformVariable var) => Value = var.Value;
    public virtual void SetValue(GameObject var) => Value = var.transform;
    public virtual void SetValue(Component var) => Value = var.transform;
    public virtual void SetNull() => Value = null;
}
