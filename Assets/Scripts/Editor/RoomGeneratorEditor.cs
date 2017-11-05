using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RoomGenerator script = (RoomGenerator)target;
        if (GUILayout.Button("Generate Grid"))
        {
            script.CreateGrid();
        }
        if (GUILayout.Button("Clean up Grid"))
        {
            script.CleanUpGrid();
        }
    }
}