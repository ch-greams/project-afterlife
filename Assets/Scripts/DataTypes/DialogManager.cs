using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class DialogManager
{
    [InlineButton("ToggleDialogPanelInEditor", "Toggle")]
    public GameObject dialogPanel;

    public Button dialogButton;
    public Image speakerIcon;
    public Text speakerName;
    public Text speech;

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<DialogId, Dialog> dialogs = new Dictionary<DialogId, Dialog>();

    private Dialog dialog;


    public void SetUp()
    {
        this.dialogButton.onClick.AddListener(this.OnDialogPanelClick);

        this.dialogs = this.dialogs
            .ToDictionary(kvp => kvp.Key, kvp => GlobalController.CreatePlayModeInstance(kvp.Value));

        // this.StartDialog(DialogId.AptN1_Bedroom_StartSpeech);
    }

    [Button]
    public void DisplayDialogFragmentInEditor()
    {
        this.DisplayDialogFragment(this.dialog.NextDialogFragment());
    }

    public void ToggleDialogPanelInEditor()
    {
        this.ToggleDialogPanel(!this.dialogPanel.activeSelf);
    }

    public void StartDialog(DialogId dialogId)
    {
        this.dialog = this.dialogs[dialogId];
        this.ToggleDialogPanel(true);
        this.DisplayNextDialogFragment();
    }

    public void OnDialogPanelClick()
    {
        this.DisplayNextDialogFragment();
    }

    private void DisplayNextDialogFragment()
    {
        DialogFragment dialogFragment = this.dialog.NextDialogFragment();

        if (dialogFragment != default(DialogFragment))
        {
            this.DisplayDialogFragment(dialogFragment);
        }
        else
        {
            this.ToggleDialogPanel(false);
        }
    }

    private void ToggleDialogPanel(bool active)
    {
        this.dialogPanel.SetActive(active);
    }

    private void DisplayDialogFragment(DialogFragment dialogFragment)
    {
        this.speakerIcon.sprite = dialogFragment.speaker.speakerIcon;
        this.speakerName.text = dialogFragment.speaker.speakerName;
        this.speech.text = dialogFragment.speech;
    }
}
