using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class Dialogue : SerializedScriptableObject
{
    public List<DialogueFragment> dialogue = new List<DialogueFragment>();

    public DialogueFragment NextDialogueFragment(ref int currentFragment)
    {
        return this.dialogue.ElementAtOrDefault(currentFragment++);
    }
}

public class DialogueFragment
{
    [InlineEditor]
    public ISpeaker speaker;
    [HideLabel, MultiLineProperty]
    public string speech;
}

public interface ISpeaker
{
    string speakerName { get; }
    Sprite speakerIcon { get; }
}
