using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class Dialog : SerializedScriptableObject
{
    public DialogId dialogId;
    public List<DialogFragment> dialog = new List<DialogFragment>();
    public int currentFragment = 0;


    public DialogFragment NextDialogFragment()
    {
        return this.dialog.ElementAtOrDefault(this.currentFragment++);
    }

    public bool IsLastDialogFragment(DialogFragment dialogFragment)
    {
        return this.dialog.Last() == dialogFragment;
    }
}

public enum DialogId
{
    AptN1_Bedroom_GetKey,
    AptN1_Bedroom_StartSpeech,
    AptN1_Bedroom_NeedKey,
}

public class DialogFragment
{
    [InlineEditor]
    public ISpeaker speaker;
    [HideLabel]
    [MultiLineProperty]
    public string speech;
}

public interface ISpeaker
{
    string speakerName { get; }
    Sprite speakerIcon { get; }
}
