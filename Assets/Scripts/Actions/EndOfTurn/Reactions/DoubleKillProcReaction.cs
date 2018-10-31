using System.Collections;


public class DoubleKillProcReaction : IEndOfTurnReaction
{
    public DoubleKillProcReactionType type = DoubleKillProcReactionType.Undefined;
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        EndOfTurnActionState endOfTurnActionState = this.globalCtrl.globalState.endOfTurnActionState;
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;

        switch (this.type)
        {
            case DoubleKillProcReactionType.Active:
                endOfTurnActionState.SetIsDoubleKillProcActive(true);
                playerActionManager.SwitchWalkProcEffect(true);
                break;
            case DoubleKillProcReactionType.Inactive:
                endOfTurnActionState.SetIsDoubleKillProcActive(false);
                playerActionManager.SwitchWalkProcEffect(false);
                break;
            case DoubleKillProcReactionType.Undefined:
            default:
                yield return null;
                break;
        }
    }
}

public enum DoubleKillProcReactionType
{
    Undefined,
    Active,
    Inactive,
}
