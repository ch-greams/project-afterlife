using System.Collections;


public class EnemyMovementReaction : IEndOfTurnReaction
{
    private EnemyManager enemyManager;


    public void Init(GlobalController globalCtrl)
    {
        this.enemyManager = globalCtrl.enemyManager;
    }

    public IEnumerator React()
    {
        yield return this.enemyManager.MoveEnemies();
    }
}
