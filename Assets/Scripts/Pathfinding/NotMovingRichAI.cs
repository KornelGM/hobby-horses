using UnityEngine;
using Pathfinding;

public class NotMovingRichAI : RichAI {
    public override void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
    {
        if (reachedEndOfPath)
        {
            velocity2D = Vector2.zero;
        }
    }
}
