using System.Collections;


public class EnemyManagerReaction : IEndOfTurnReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        EnemyManager enemyManager = this.globalCtrl.enemyManager;

        yield return enemyManager.MoveEnemies();

        enemyManager.SpawnEnemies();
    }
}
