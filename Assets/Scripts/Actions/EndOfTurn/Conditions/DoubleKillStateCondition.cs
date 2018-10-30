

public class DoubleKillStateCondition : IEndOfTurnCondition
{
    public DoubleKillStateConditionType type = DoubleKillStateConditionType.Undefined;

    private EndOfTurnActionState endOfTurnActionState;


    public void Init(GlobalController globalCtrl)
    {
        this.endOfTurnActionState = globalCtrl.globalState.endOfTurnActionState;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case DoubleKillStateConditionType.Active:
                return this.endOfTurnActionState.isDoubleKillProcActive;
            case DoubleKillStateConditionType.Inactive:
                return !this.endOfTurnActionState.isDoubleKillProcActive;
            case DoubleKillStateConditionType.Undefined:
            default:
                return false;
        }
    }
}

public enum DoubleKillStateConditionType
{
    Undefined,
    Active,
    Inactive,
}
