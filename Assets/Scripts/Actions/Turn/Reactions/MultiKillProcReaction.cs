using System.Collections;


public class MultiKillProcReaction : IEndOfTurnReaction
{
    public int turnsTillReset = 1;
    public int amountForProc = 2;
    public int scoreValue = 0;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        GlobalState globalState = this.globalCtrl.globalState;
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;

        if (globalState.GetBooleanParameterFromState("isDoubleKillProcActive"))
        {
            globalState.SetBooleanParameterInState("isDoubleKillProcActive", false);
            playerActionManager.SwitchWalkProcEffect(false);
        }
        else
        {
            globalState.SetIntegerParameterInState(
                "turnsTillResetLeft",
                globalState.GetIntegerParameterFromState("turnsTillResetLeft") - 1
            );

            if (globalState.GetIntegerParameterFromState("enemiesKilled") >= this.amountForProc)
            {
                globalState.SetBooleanParameterInState("isDoubleKillProcActive", true);
                playerActionManager.SwitchWalkProcEffect(true);

                this.globalCtrl.statsManager.IncrementScore(this.scoreValue);
            }
        }

        if (
            (globalState.GetIntegerParameterFromState("turnsTillResetLeft") <= 0) ||
            (globalState.GetIntegerParameterFromState("enemiesKilled") == 0)
        ) {
            globalState.SetIntegerParameterInState("enemiesKilled", 0);
            globalState.SetIntegerParameterInState("turnsTillResetLeft", this.turnsTillReset);
        }

        yield return null;
    }
}
