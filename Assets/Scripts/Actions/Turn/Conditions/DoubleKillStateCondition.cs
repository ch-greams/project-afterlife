

public class DoubleKillStateCondition : ITurnActionCondition
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
                return this.globalState.GetBooleanParameterFromState("isDoubleKillProcActive");
            case DoubleKillStateConditionType.Inactive:
                return !this.globalState.GetBooleanParameterFromState("isDoubleKillProcActive");
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
