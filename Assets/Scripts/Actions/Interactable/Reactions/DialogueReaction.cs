using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class DialogueReaction : IInteractableReaction
{
    [Required]
    public DialogueCollection dialogueCollection;

    [ValueDropdown("dialogueTitles")]
    public string dialogueTitle;


    private List<string> dialogueTitles
    {
        get
        {
            return (this.dialogueCollection != null)
                ? new List<string>(this.dialogueCollection.dialogues.Keys)
                : new List<string>();
        }
    }

    private DialogueManager dialogueManager;


    public void Init(Interactable interactable)
    {
        this.dialogueManager = interactable.sceneCtrl.globalCtrl.dialogueManager;
    }

    public IEnumerator React()
    {
        Dialogue dialogue = this.dialogueCollection.dialogues[this.dialogueTitle];
        yield return this.dialogueManager.StartDialogueAsync(dialogue);
    }
}
