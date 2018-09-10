using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyGenerator : SerializedMonoBehaviour
{
    public GameObject shadowPrefab;

    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Point> enemyPositions = new List<Point>();

    public float visibleRange = 3.0F;

    private GlobalController globalCtrl;
    private EnemyManager enemyManager;


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.enemyManager = this.globalCtrl.enemyManager;
    }

    [Button("Spawn Shadows", ButtonSizes.Medium)]
    public void SpawnShadows()
    {
        List<Tile> tiles = this.globalCtrl.sceneCtrl.tiles;

        foreach (Point point in this.enemyPositions)
        {
            Tile tile = tiles.Find(t => t.point == point);
            GameObject obj = Instantiate(this.shadowPrefab, tile.obj.transform.position, Quaternion.identity);

            Enemy enemy = new Enemy("Enemy " + point.ToString(), this.visibleRange, obj.transform, tile);
            this.enemyManager.enemies.Add(enemy);
        }
    }

    // TODO: Move to Scene or Global controller
    [Button("Next Turn", ButtonSizes.Medium)]
    public void NextTurn()
    {
        this.globalCtrl.NextTurn();
    }
}