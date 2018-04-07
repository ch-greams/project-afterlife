﻿using System.Collections;
using Sirenix.OdinInspector;


public class DialogueReaction : IReaction
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
        this.dialogueManager.StartDialogue(this.dialogue);

        yield return null;
    }
}