﻿using System.Collections;


public class EnemyManagerReaction : ITurnActionReaction
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

        yield return enemyManager.SpawnEnemies();
    }
}
