using System.Collections;


public class InteractionReaction : IEndOfTurnReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        yield return this.globalCtrl.playerActionManager.currentInteractable.OnClickAsync();
    }
}
