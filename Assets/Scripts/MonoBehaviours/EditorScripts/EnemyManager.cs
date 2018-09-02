using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyManager : SerializedMonoBehaviour
{
    public SceneController sceneCtrl;
    public GameObject shadowPrefab;

    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Point> enemyPositions = new List<Point>();

    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Enemy> enemies = new List<Enemy>();


    [Button("Spawn Shadows", ButtonSizes.Medium)]
    public void SpawnShadows()
    {
        foreach (Point point in this.enemyPositions)
        {
            Tile tile = this.sceneCtrl.tiles.Find(t => t.point == point);
            GameObject obj = Instantiate(this.shadowPrefab, tile.obj.transform.position, Quaternion.identity);

            enemies.Add(new Enemy("Enemy " + point.ToString(), obj.transform, tile));
        }
    }

    [Button("Next Turn", ButtonSizes.Medium)]
    public void NextTurn()
    {
        StartCoroutine(this.MoveEnemies());
    }


    private IEnumerator MoveEnemies()
    {
        foreach (Enemy enemy in this.enemies)
        {
            yield return enemy.MoveToPlayer(this.sceneCtrl.player);
        }   
    }
}