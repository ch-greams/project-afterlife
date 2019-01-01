using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class Character : ScriptableObject, ISpeaker
{
    public string label;
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public Sprite icon;

    public string speakerName { get { return this.label; } }
    public Sprite speakerIcon { get { return this.icon; } }
}
