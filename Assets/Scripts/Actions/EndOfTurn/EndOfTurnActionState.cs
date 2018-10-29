

public class EndOfTurnActionState
{
    public EnemyKillConditionState enemyKillConditionState;


    public EndOfTurnActionState()
    {
        this.enemyKillConditionState = new EnemyKillConditionState();
    }
}


public class EnemyKillConditionState
{
    public int enemiesKilled = 0;
    public int turnsTillResetLeft = 0;


    public EnemyKillConditionState()
    {
        this.enemiesKilled = 0;
        this.turnsTillResetLeft = 0;
    }


    public int SetEnemiesKilled(int enemiesKilled)
    {
        this.enemiesKilled = enemiesKilled;
        return this.enemiesKilled;
    }

    public int IncreaseEnemiesKilled(int deltaValue)
    {
        return this.SetEnemiesKilled(this.enemiesKilled + deltaValue);
    }

    public int SetTurnsTillResetLeft(int turnsTillResetLeft)
    {
        this.turnsTillResetLeft = turnsTillResetLeft;
        return this.turnsTillResetLeft;
    }

    public int DecreaseTurnsTillResetLeft(int deltaValue)
    {
        return this.SetTurnsTillResetLeft(this.turnsTillResetLeft - deltaValue);
    }
}