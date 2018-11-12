

public class DoubleKillStateCondition : IEndOfTurnCondition
{
    public DoubleKillStateConditionType type = DoubleKillStateConditionType.Undefined;

    private GlobalState globalState;


    public void Init(GlobalController globalCtrl)
    {
        this.globalState = globalCtrl.globalState;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case DoubleKillStateConditionType.Active:
                return this.globalState.GetVariableFromState<bool>("isDoubleKillProcActive");
            case DoubleKillStateConditionType.Inactive:
                return !this.globalState.GetVariableFromState<bool>("isDoubleKillProcActive");
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
