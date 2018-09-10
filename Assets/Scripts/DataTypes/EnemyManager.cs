using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyManager
{
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Enemy> enemies = new List<Enemy>();


    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator OnTurnChange()
    {
        yield return this.MoveEnemies();
    }

    private IEnumerator MoveEnemies()
    {
        this.enemies.FindAll(this.EnemyInSight).ForEach((enemy) => { enemy.isLockedOnPlayer = true; });

        List<Enemy> enemiesLockedOnPlayer = this.enemies.FindAll((enemy) => (enemy.isLockedOnPlayer));
        
        foreach (Enemy enemy in enemiesLockedOnPlayer)
        {
            yield return enemy.MoveToPlayer(this.globalCtrl.sceneCtrl.player);
        }   
    }

    private bool EnemyInSight(Enemy enemy)
    {
        Tile playerTile = this.globalCtrl.sceneCtrl.player.tile;
        return enemy.tile.FindPathFrom(playerTile, (t) => (t.isVisible)) != null;
    }

    public void TryKillEnemyOnPoint(Point point)
    {
        Enemy enemy = this.enemies.Find((e) => e.tile.point == point);

        if (enemy != null)
        {
            this.KillEnemy(enemy);
        }
    }

    private void KillEnemy(Enemy enemy)
    {
        GameObject.DestroyImmediate(enemy.characterTransform.gameObject);
        enemy.tile.RefreshTileState(enemy.tile.isVisible, false);
        this.enemies.Remove(enemy);
    }
}
