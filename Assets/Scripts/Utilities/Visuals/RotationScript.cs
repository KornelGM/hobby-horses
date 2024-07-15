using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RotateOnAxis : MonoBehaviour
{
    public RotationAxis rotationAxis = RotationAxis.Y; // Axis of rotation
    public float rotationAmountPerSecond = 90f;         // Amount of rotation in degrees per second
    [SerializeField] private bool infiniteRotation = false; // Checkbox for infinite rotation
    [SerializeField] private float rotationTimeframe = 2f;  // Timeframe length for rotation in seconds
    private float remainingRotationTime;
    private bool wasInfiniteRotation;

    private void Start()
    {
        remainingRotationTime = rotationTimeframe;
        wasInfiniteRotation = infiniteRotation;
    }

    private void Update()
    {
        if (infiniteRotation || remainingRotationTime > 0f)
        {
            // Calculate the rotation angle for this frame based on rotation amount per second
            float rotationAngle = rotationAmountPerSecond * Time.deltaTime;

            // Rotate the object around the selected axis
            transform.Rotate(GetRotationAxisVector(rotationAxis), rotationAngle);

            // Reduce the remaining rotation timeframe and stop rotating when the timeframe is over
            if (!infiniteRotation && !wasInfiniteRotation)
            {
                remainingRotationTime -= Time.deltaTime;
                if (remainingRotationTime <= 0f)
                {
                    enabled = false; // Disable the script to stop further rotation
                }
            }
        }

        wasInfiniteRotation = infiniteRotation;
    }

    private Vector3 GetRotationAxisVector(RotationAxis axis)
    {
        switch (axis)
        {
            case RotationAxis.X:
                return Vector3.right;
            case RotationAxis.Y:
                return Vector3.up;
            case RotationAxis.Z:
                return Vector3.forward;
            default:
                return Vector3.up;
        }
    }

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RotateOnAxis))]
public class RotateOnAxisEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty rotationAxis = serializedObject.FindProperty("rotationAxis");
        SerializedProperty rotationAmountPerSecond = serializedObject.FindProperty("rotationAmountPerSecond");
        SerializedProperty infiniteRotation = serializedObject.FindProperty("infiniteRotation");
        SerializedProperty rotationTimeframe = serializedObject.FindProperty("rotationTimeframe");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotation Axis");
        rotationAxis.enumValueIndex = (int)(RotateOnAxis.RotationAxis)EditorGUILayout.EnumPopup((RotateOnAxis.RotationAxis)rotationAxis.enumValueIndex);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(rotationAmountPerSecond);
        EditorGUILayout.PropertyField(infiniteRotation);

        if (infiniteRotation.boolValue)
        {
            EditorGUILayout.LabelField("Rotation Timeframe", "Infinite");
        }
        else
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            rotationTimeframe.floatValue = EditorGUILayout.FloatField("Rotation Timeframe", rotationTimeframe.floatValue);
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
