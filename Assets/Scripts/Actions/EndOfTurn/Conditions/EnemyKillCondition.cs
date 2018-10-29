

public class EnemyKillCondition : IEndOfTurnCondition
{
    public EnemyKillConditionType type = EnemyKillConditionType.Undefined;

    public const int TURNS_TILL_RESET = 2;
    public const int KILL_AMOUNT_FOR_PROC = 2;

    private EnemyKillConditionState enemyKillConditionState;


    public void Init(GlobalController globalCtrl)
    {
        this.enemyKillConditionState = globalCtrl.globalState.endOfTurnActionState.enemyKillConditionState;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case EnemyKillConditionType.ValidProcCondition:
                return (this.enemyKillConditionState.enemiesKilled >= KILL_AMOUNT_FOR_PROC);
            case EnemyKillConditionType.ResetConditionState:
                return (
                    (this.enemyKillConditionState.turnsTillResetLeft <= 0) ||
                    (this.enemyKillConditionState.enemiesKilled == 0)
                );
            case EnemyKillConditionType.Undefined:
            default:
                return false;
        }
    }
}

public enum EnemyKillConditionType
{
    Undefined,
    ValidProcCondition,
    ResetConditionState,
}
