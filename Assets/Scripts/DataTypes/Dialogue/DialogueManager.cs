using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System;


public class DialogueManager
{
    [InlineButton("ToggleDialoguePanelInEditor", "Toggle")]
    public GameObject dialoguePanel;

    public Button dialogueButton;
    public Image speakerIcon;
    public Text speakerName;
    public Text speech;

    private Dialogue dialogue;
    private int currentFragment = 0;
    private bool isDialogueInProgress = false;


    public void Init()
    {
        this.dialogueButton.onClick.AddListener(this.DisplayNextDialogueFragment);
    }

    public void InputListener()
    {
        if (this.isDialogueInProgress)
        {
            bool IsXboxJoystick = Array.Exists(
                Input.GetJoystickNames(),
                (joystick) => (joystick != null) && joystick.ToLower().Contains("xbox")
            );

            if (Input.GetButtonDown(IsXboxJoystick ? "Button A" : "Button B"))
            {
                this.DisplayNextDialogueFragment();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        this.currentFragment = 0;

        this.ToggleDialoguePanel(true);
        this.DisplayNextDialogueFragment();
    }

    public IEnumerator StartDialogueAsync(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        this.currentFragment = 0;
        this.isDialogueInProgress = true;

        this.ToggleDialoguePanel(true);
        this.DisplayNextDialogueFragment();

        while (this.isDialogueInProgress)
        {
            yield return null;
        }
    }

    private void DisplayNextDialogueFragment()
    {
        DialogueFragment dialogueFragment = this.dialogue.PlayNextDialogueFragment(ref this.currentFragment);

        if (dialogueFragment != default(DialogueFragment))
        {
            this.DisplayDialogueFragment(dialogueFragment);
        }
        else
        {
            this.ToggleDialoguePanel(false);
            this.isDialogueInProgress = false;
        }
    }

    private void ToggleDialoguePanel(bool active)
    {
        this.dialoguePanel.SetActive(active);
    }

    private void DisplayDialogueFragment(DialogueFragment dialogueFragment)
    {
        this.speakerIcon.sprite = dialogueFragment.speaker.speakerIcon;
        this.speakerName.text = dialogueFragment.speaker.speakerName;
        this.speech.text = dialogueFragment.speech;
    }

#if UNITY_EDITOR

    public void ToggleDialoguePanelInEditor()
    {
        this.ToggleDialoguePanel(!this.dialoguePanel.activeSelf);
    }

#endif
}
