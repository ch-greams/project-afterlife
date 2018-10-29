using System.Collections;


public class EnemyKillStateReaction : IEndOfTurnReaction
{
    public EnemyKillStateReactionType type = EnemyKillStateReactionType.Undefined;

    public const int TURNS_TILL_RESET = 2;

    private EnemyKillConditionState enemyKillConditionState;


    public void Init(EndOfTurnActionState endOfTurnActionState)
    {
        this.enemyKillConditionState = endOfTurnActionState.enemyKillConditionState;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case EnemyKillStateReactionType.ResetEnemyKillState:
                this.enemyKillConditionState.SetEnemiesKilled(0);
                this.enemyKillConditionState.SetTurnsTillResetLeft(TURNS_TILL_RESET);
                break;
            case EnemyKillStateReactionType.DecrementTurnsTillResetLeft:
                this.enemyKillConditionState.DecreaseTurnsTillResetLeft(1);
                break;
            case EnemyKillStateReactionType.Undefined:
            default:
                yield return null;
                break;
        }
    }
}

public enum EnemyKillStateReactionType
{
    Undefined,
    ResetEnemyKillState,
    DecrementTurnsTillResetLeft,
}
