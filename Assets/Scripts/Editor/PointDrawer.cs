using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Point))]
public class PointDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect xRectLabel = new Rect(position.x -1, position.y, 12, position.height);
        Rect xRect = new Rect(position.x + 12, position.y, 64, position.height);
        Rect yRectLabel = new Rect(position.x + 78, position.y, 12, position.height);
        Rect yRect = new Rect(position.x + 91, position.y, 64, position.height);

        EditorGUI.LabelField(xRectLabel, "X");
        EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.LabelField(yRectLabel, "Y");
        EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}