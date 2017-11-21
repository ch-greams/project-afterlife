using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;


[OdinDrawer]
public class PointDrawer : OdinValueDrawer<Point>
{
    protected override void DrawPropertyLayout(IPropertyValueEntry<Point> entry, GUIContent label)
    {
        Point value = entry.SmartValue;

        Rect rect = EditorGUILayout.GetControlRect();

        // In Odin, labels are optional and can be null, so we have to account for that.
        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        float prev = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 20;

        value.x = EditorGUI.IntSlider(rect.AlignLeft(rect.width * 0.5f), "X", value.x, 0, 50);
        value.y = EditorGUI.IntSlider(rect.AlignRight(rect.width * 0.5f), "Y", value.y, 0, 50);

        EditorGUIUtility.labelWidth = prev;

        entry.SmartValue = value;
    }
}