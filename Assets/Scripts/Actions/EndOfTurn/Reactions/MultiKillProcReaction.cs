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

        if (globalState.GetVariableFromState<bool>("isDoubleKillProcActive"))
        {
            globalState.SetVariableInState("isDoubleKillProcActive", false);
            playerActionManager.SwitchWalkProcEffect(false);
        }
        else
        {
            globalState.SetVariableInState(
                "turnsTillResetLeft",
                globalState.GetVariableFromState<int>("turnsTillResetLeft") - 1
            );

            if (globalState.GetVariableFromState<int>("enemiesKilled") >= this.amountForProc)
            {
                globalState.SetVariableInState("isDoubleKillProcActive", true);
                playerActionManager.SwitchWalkProcEffect(true);

                this.globalCtrl.statsManager.IncrementScore(this.scoreValue);
            }
        }

        if (
            (globalState.GetVariableFromState<int>("turnsTillResetLeft") <= 0) ||
            (globalState.GetVariableFromState<int>("enemiesKilled") == 0)
        ) {
            globalState.SetVariableInState("enemiesKilled", 0);
            globalState.SetVariableInState("turnsTillResetLeft", this.turnsTillReset);
        }

        yield return null;
    }
}
