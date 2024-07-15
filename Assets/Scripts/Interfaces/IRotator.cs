using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterRotator
{
    /// <summary>
    /// Call in update
    /// </summary>
    /// <param name="MouseX"></param>
    public void Rotate(float MouseX);
    public void RotateTo(Quaternion rotation);
}
