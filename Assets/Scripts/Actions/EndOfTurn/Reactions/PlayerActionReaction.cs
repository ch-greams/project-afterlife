using System.Collections;


public class PlayerActionReaction : IEndOfTurnReaction
{
    private PlayerActionManager playerActionManager;


    public void Init(GlobalController globalCtrl)
    {
        this.playerActionManager = globalCtrl.playerActionManager;
    }

    public IEnumerator React()
    {
        yield return this.playerActionManager.TriggerSelectedAction();
    }
}
