using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform parentTransform;    // Reference to the parent transform (formerly Cog1 transform)
    public RotationAxis rotationAxis = RotationAxis.Y; // Axis to consider for rotation
    public float rotationMultiplier = 1f; // Multiplier for the rotation amount

    private float totalRotation = 0f; // Total accumulated rotation

    private void Update()
    {
        if (parentTransform != null && rotationMultiplier != 0f)
        {
            // Get the desired rotation component based on the selected axis
            float rotationComponent = 0f;
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    rotationComponent = parentTransform.rotation.eulerAngles.x;
                    break;
                case RotationAxis.Y:
                    rotationComponent = parentTransform.rotation.eulerAngles.y;
                    break;
                case RotationAxis.Z:
                    rotationComponent = parentTransform.rotation.eulerAngles.z;
                    break;
            }

            // Calculate the rotation based on the multiplier and time
            float rotationAmount = rotationComponent * rotationMultiplier * Time.deltaTime;
            totalRotation += rotationAmount;

            // Apply the total rotation to the target object's selected axis
            Vector3 newEulerAngles = transform.eulerAngles;
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    newEulerAngles.x = totalRotation;
                    break;
                case RotationAxis.Y:
                    newEulerAngles.y = totalRotation;
                    break;
                case RotationAxis.Z:
                    newEulerAngles.z = totalRotation;
                    break;
            }
            transform.eulerAngles = newEulerAngles;
        }
    }

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
}
