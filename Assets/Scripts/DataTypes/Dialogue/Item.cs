using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class Item : ScriptableObject, ISpeaker
{
    public ItemId id;
    public string label;
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public Sprite icon;

    public string speakerName { get { return this.label; } }
    public Sprite speakerIcon { get { return this.icon; } }
}

public enum ItemId
{
    AptN1_Bedroom_DoorKey,
}