using System.Collections;


public class DialogReaction : IReaction
{
    public DialogId dialogId;
    private DialogManager dialogManager;


    public void Init(Interactable interactable)
    {
        this.dialogManager = interactable.sceneCtrl.globalCtrl.dialogManager;
    }
    public IEnumerator React()
    {
        this.dialogManager.StartDialog(this.dialogId);

        yield return null;
    }
}
