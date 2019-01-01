using System.Collections;
using Sirenix.OdinInspector;


public class DialogueReaction : IInteractableReaction
{
    [Required]
    public Dialogue dialogue;
    private DialogueManager dialogueManager;


    public void Init(Interactable interactable)
    {
        this.dialogueManager = interactable.sceneCtrl.globalCtrl.dialogueManager;
    }

    public IEnumerator React()
    {
        yield return this.dialogueManager.StartDialogueAsync(this.dialogue);
    }
}
