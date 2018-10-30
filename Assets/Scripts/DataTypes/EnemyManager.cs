using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class EnemyManager
{
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Enemy> enemies = new List<Enemy>();
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        // TODO: Load from sceneState
    }


    public void SpawnEnemies()
    {
        foreach (EnemySpawnPoint esp in this.enemySpawnPoints)
        {
            Enemy enemy = esp.TrySpawnEnemy(this.globalCtrl);
            
            if (enemy != null)
            {
                this.enemies.Add(enemy);
            }
        }
    }

    public IEnumerator MoveEnemies()
    {
        this.enemies.FindAll(this.EnemyInSight).ForEach((enemy) => { enemy.isLockedOnPlayer = true; });

        List<Enemy> enemiesLockedOnPlayer = this.enemies.FindAll((enemy) => (enemy.isLockedOnPlayer));
        
        foreach (Enemy enemy in enemiesLockedOnPlayer)
        {
            yield return enemy.MoveToPlayer(this.globalCtrl.sceneCtrl.player, this);
        }   
    }

    private bool EnemyInSight(Enemy enemy)
    {
        Tile playerTile = this.globalCtrl.sceneCtrl.player.tile;
        return enemy.tile.FindPathFrom(playerTile, (t) => (t.isVisible)) != null;
    }

    public bool TryDestroyEnemyOnPoint(Point point)
    {
        Enemy enemy = this.enemies.Find((e) => e.tile.point == point);

        if (enemy != null)
        {
            enemy.Destroy();
            this.enemies.Remove(enemy);

            return true;
        }

        return false;
    }
}
