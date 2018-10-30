using System.Collections;


public class DoubleKillProcReaction : IEndOfTurnReaction
{
    public DoubleKillProcReactionType type = DoubleKillProcReactionType.Undefined;
    private EndOfTurnActionState endOfTurnActionState;


    public void Init(GlobalController globalCtrl)
    {
        this.endOfTurnActionState = globalCtrl.globalState.endOfTurnActionState;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case DoubleKillProcReactionType.Active:
                this.endOfTurnActionState.SetIsDoubleKillProcActive(true);
                break;
            case DoubleKillProcReactionType.Inactive:
                this.endOfTurnActionState.SetIsDoubleKillProcActive(false);
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
