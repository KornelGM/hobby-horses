#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ClampedValue)), CanEditMultipleObjects]
public class ClampedValuePropertyDrawer : PropertyDrawer
{
    public SerializedProperty _value, _minValue, _maxValue, _valueChanged;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        _minValue = property.FindPropertyRelative("_minValue");
        _maxValue = property.FindPropertyRelative("_maxValue");
        _value = property.FindPropertyRelative("_value");
        
        _value.floatValue = EditorGUI.Slider(position, _value.floatValue, _minValue.floatValue, _maxValue.floatValue);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
#endif