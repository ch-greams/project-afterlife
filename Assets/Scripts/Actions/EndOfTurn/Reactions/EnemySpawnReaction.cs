using System.Collections;


public class EnemySpawnReaction : IEndOfTurnReaction
{
    private EnemyManager enemyManager;


    public void Init(GlobalController globalCtrl)
    {
        this.enemyManager = globalCtrl.enemyManager;
    }

    public IEnumerator React()
    {
        this.enemyManager.SpawnEnemies();

        yield return null;
    }
}
