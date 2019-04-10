using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyManager : SerializedMonoBehaviour
{
    [ListDrawerSettings(DraggableItems = false)]
    public List<Enemy> enemies = new List<Enemy>();
    [ListDrawerSettings(DraggableItems = false)]
    public List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        // TODO: Load from sceneState
    }


    public IEnumerator SpawnEnemies()
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;

        foreach (EnemySpawnPoint esp in this.enemySpawnPoints)
        {
            Enemy enemy = esp.TrySpawnEnemy(sceneCtrl.tiles, sceneCtrl);
            
            if (enemy != null)
            {
                this.enemies.Add(enemy);

                sceneCtrl.sceneState.enemies.Add(enemy.state.id, enemy.state);

                yield return enemy.OnSpawn();
            }
        }
    }

    public IEnumerator MoveEnemies()
    {
        this.enemies.FindAll(this.EnemyInSight).ForEach((enemy) => { enemy.state.isLockedOn = true; });
      
        Dictionary<bool, List<Enemy>> enemiesByLockedOnValue = this.enemies
            .GroupBy(e => e.state.isLockedOn)
            .ToDictionary(group => group.Key, group => group.ToList());

        if (enemiesByLockedOnValue.ContainsKey(true))
        {
            foreach (Enemy enemy in enemiesByLockedOnValue[true])
            {
                yield return enemy.MoveToPlayer(this.globalCtrl.sceneCtrl.player, this);
            }
        }
        
        if (enemiesByLockedOnValue.ContainsKey(false))
        {
            foreach (Enemy enemy in enemiesByLockedOnValue[false])
            {
                yield return enemy.MoveToRandomNeighbourTile(this.globalCtrl.sceneCtrl.player, this);
            }
        }
    }

    private bool EnemyInSight(Enemy enemy)
    {
        Tile playerTile = this.globalCtrl.sceneCtrl.player.tile;
        return enemy.tile.FindPathFrom(playerTile, (t) => (t.isVisible)) != null;
    }

    public IEnumerator TryDestroyEnemyOnPoint(Point point, bool byPlayer)
    {
        Enemy enemy = this.enemies.Find((e) => e.tile.point == point);

        if (enemy != null)
        {
            this.enemies.Remove(enemy);
            this.globalCtrl.sceneCtrl.sceneState.enemies.Remove(enemy.state.id);

            yield return enemy.Destroy();

            if (byPlayer)
            {
                // TODO: Move into deathActions list
                this.globalCtrl.globalState.SetIntegerParameterInState(
                    "enemiesKilled",
                    (this.globalCtrl.globalState.GetIntegerParameterFromState("enemiesKilled") + 1)
                );
                this.globalCtrl.statsManager.IncrementScore(10);

                if (enemy.state.itemDropRate > Random.Range(0F, 1F))
                {
                    this.globalCtrl.collectableManager.TrySpawnItem(point, "Healthpack");
                }
            }
        }
    }
}
