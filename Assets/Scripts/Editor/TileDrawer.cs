using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;


[OdinDrawer]
public class TileDrawer : OdinValueDrawer<Tile>
{
    protected override void DrawPropertyLayout(IPropertyValueEntry<Tile> entry, GUIContent label)
    {
        if (entry.SmartValue == null)
        {
            entry.SmartValue = new Tile();
        }

        SirenixEditorGUI.BeginBox(label);

        entry.Property.Children["passable"].Draw();
        entry.Property.Children["point"].Draw();
        entry.Property.Children["obj"].Draw();

        // entry.Property.Children["allNeighbours"].Draw();
        // entry.Property.Children["neighbours"].Draw();

        SirenixEditorGUI.EndBox();
    }
}
