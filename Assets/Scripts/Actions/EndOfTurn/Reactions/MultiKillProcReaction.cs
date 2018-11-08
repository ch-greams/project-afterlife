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

        EndOfTurnActionState endOfTurnActionState = globalState.endOfTurnActionState;
        EnemyKillConditionState enemyKillConditionState = globalState.endOfTurnActionState.enemyKillConditionState;


        if (endOfTurnActionState.isDoubleKillProcActive)
        {
            endOfTurnActionState.SetIsDoubleKillProcActive(false);
            playerActionManager.SwitchWalkProcEffect(false);
        }
        else
        {
            enemyKillConditionState.DecreaseTurnsTillResetLeft(1);

            if (enemyKillConditionState.enemiesKilled >= this.amountForProc)
            {
                endOfTurnActionState.SetIsDoubleKillProcActive(true);
                playerActionManager.SwitchWalkProcEffect(true);

                this.globalCtrl.scoreManager.IncrementScore(this.scoreValue);
            }
        }

        if ((enemyKillConditionState.turnsTillResetLeft <= 0) || (enemyKillConditionState.enemiesKilled == 0))
        {
            enemyKillConditionState.SetEnemiesKilled(0);
            enemyKillConditionState.SetTurnsTillResetLeft(this.turnsTillReset);
        }

        yield return null;
    }
}
