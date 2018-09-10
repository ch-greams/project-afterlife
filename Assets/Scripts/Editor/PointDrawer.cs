using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;


public class PointDrawer : OdinValueDrawer<Point>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Point value = this.ValueEntry.SmartValue;
        Rect rect = EditorGUILayout.GetControlRect();

        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        float prev = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 20;

        value.x = EditorGUI.IntSlider(rect.AlignLeft(rect.width * 0.5f), "X", value.x, 0, 100);
        value.y = EditorGUI.IntSlider(rect.AlignRight(rect.width * 0.5f), "Y", value.y, 0, 100);

        EditorGUIUtility.labelWidth = prev;
        this.ValueEntry.SmartValue = value;
    }
}