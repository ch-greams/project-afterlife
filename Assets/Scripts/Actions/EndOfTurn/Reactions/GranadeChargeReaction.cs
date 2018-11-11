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
        EndOfTurnActionState endOfTurnActionState = this.globalCtrl.globalState.endOfTurnActionState;
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;

        switch (this.type)
        {
            case GranadeChargeReactionType.PutOnCooldown:
                endOfTurnActionState.turnsTillGranadeChargeLeft = this.abilityCooldown;
                playerActionManager.GranadeChargeEffect(endOfTurnActionState.turnsTillGranadeChargeLeft);
                break;
            case GranadeChargeReactionType.ReduceCooldown:
                endOfTurnActionState.turnsTillGranadeChargeLeft--;
                playerActionManager.GranadeChargeEffect(endOfTurnActionState.turnsTillGranadeChargeLeft);
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
