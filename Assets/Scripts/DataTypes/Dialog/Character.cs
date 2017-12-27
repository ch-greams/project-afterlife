using UnityEngine;


[CreateAssetMenu]
public class Character : ScriptableObject, ISpeaker
{
    public CharacterId id;
    public string label;
    public Sprite icon;

    public string speakerName { get { return this.label; } }
    public Sprite speakerIcon { get { return this.icon; } }
}

public enum CharacterId
{
    Hero,
    Guide,
}