using System.Collections;
using Sirenix.OdinInspector;


public class GranadeChargeReaction : IEndOfTurnReaction
{
    public GranadeChargeReactionType type = GranadeChargeReactionType.Undefined;

    [ShowIf("type", GranadeChargeReactionType.PutOnCooldown)]
    public int abilityCooldown = 3;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        GlobalState globalState = this.globalCtrl.globalState;
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;

        switch (this.type)
        {
            case GranadeChargeReactionType.PutOnCooldown:
                globalState.SetVariableInState("turnsTillGranadeChargeLeft", this.abilityCooldown);
                playerActionManager.GranadeChargeEffect(globalState.GetVariableFromState<int>("turnsTillGranadeChargeLeft"));
                break;
            case GranadeChargeReactionType.ReduceCooldown:
                globalState.SetVariableInState(
                    "turnsTillGranadeChargeLeft",
                    (globalState.GetVariableFromState<int>("turnsTillGranadeChargeLeft") - 1)
                );
                playerActionManager.GranadeChargeEffect(globalState.GetVariableFromState<int>("turnsTillGranadeChargeLeft"));
                break;
            case GranadeChargeReactionType.Undefined:
            default:
                yield return null;
                break;
        }
    }
}

public enum GranadeChargeReactionType
{
    Undefined,
    ReduceCooldown,
    PutOnCooldown,
}
