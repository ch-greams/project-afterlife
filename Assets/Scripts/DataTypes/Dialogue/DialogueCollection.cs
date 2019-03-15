using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class DialogueCollection : SerializedScriptableObject
{
    [BoxGroup("File Import & Export")]
    public TextAsset textFile;


    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();



    [BoxGroup("File Import & Export"), Button(ButtonSizes.Medium, Name = "Import")]
    private void ImportFromFile()
    {
        this.dialogues = new Dictionary<string, Dialogue>();
        
        string textDialogueCollection = this.textFile.ToString();
        string[] textDialogueLines = Regex.Split(textDialogueCollection, "(---[a-zA-Z]+---(\r\n|\r|\n))");
        string nextTitle = "";

        foreach (string textDialogueLine in textDialogueLines)
        {
            if (!string.IsNullOrWhiteSpace(textDialogueLine))
            {
                if (string.IsNullOrWhiteSpace(nextTitle))
                {
                    nextTitle = Regex.Replace(textDialogueLine, "---|\r\n|\r|\n", "");
                }
                else
                {
                    this.dialogues.Add(nextTitle, new Dialogue(textDialogueLine));
                    nextTitle = "";
                }
            }
        }
    }
}
