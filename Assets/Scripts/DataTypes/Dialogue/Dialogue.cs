using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


// NOTE: Double colon "::" for speaker-speech separation
// NOTE: Double semi-colon ";;" for small pause/delay during dialog display
public class Dialogue
{
    [ListDrawerSettings(Expanded = true, ListElementLabelName = "preview")]
    public List<DialogueFragment> dialogueFragments = new List<DialogueFragment>();

    // TODO: Move 'currentFragment' here?


    public Dialogue()
    {
        this.dialogueFragments = new List<DialogueFragment>();
    }

    public Dialogue(string textDialogue)
    {
        string[] textDialogueFragments = Regex.Split(textDialogue, "\r\n|\r|\n");

        foreach (string fragment in textDialogueFragments)
        {
            if (!string.IsNullOrWhiteSpace(fragment))
            {
                string[] fragmentParts = Regex.Split(fragment, "(::)");
                Character characterAsset = this.FindSpeaker(fragmentParts[0]);

                this.AddDialogueFragment(characterAsset, fragmentParts[2]);
            }
        }
    }


    public DialogueFragment PlayNextDialogueFragment(ref int currentFragment)
    {
        return this.dialogueFragments.ElementAtOrDefault(currentFragment++);
    }


    private void AddDialogueFragment(ISpeaker speaker, string speech)
    {
        this.dialogueFragments.Add(new DialogueFragment(speaker, speech));
    }


    private Character FindSpeaker(string name)
    {
        string characterFilePath = string.Format("Assets/Scriptable Assets/Speakers/Characters/{0}.asset", name);
        return AssetDatabase.LoadAssetAtPath<Character>(characterFilePath);
    }
}

public class DialogueFragment
{
    [InlineEditor]
    public ISpeaker speaker;
    [HideLabel, MultiLineProperty]
    public string speech;

    private string preview
    {
        get
        {
            string speechPreview = (speech.Length > 16) ? speech.Substring(0, 16) + "..." : speech;
            return string.Format("{0}: {1}", speaker.speakerName, speechPreview);
        }
    }

    public DialogueFragment(ISpeaker speaker, string speech)
    {
        this.speaker = speaker;
        this.speech = speech;
    }
}

public interface ISpeaker
{
    string speakerName { get; }
    Sprite speakerIcon { get; }
}
