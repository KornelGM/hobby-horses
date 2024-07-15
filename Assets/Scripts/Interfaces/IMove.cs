using UnityEngine;

public interface IMove
{
    /// <summary>
    /// Call in update
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="isSprinting"></param>
    public void Move(Vector3 moveInput, bool isSprinting);
}
