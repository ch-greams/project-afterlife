using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class PlaygroundGenerator : SerializedMonoBehaviour
{
    [FoldoutGroup("Enemy Generator")]
    public GameObject shadowPrefab;

    [FoldoutGroup("Enemy Generator")]
    public float movementSpeed = 3.0F;

    [FoldoutGroup("Enemy Generator")]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Point> enemyPositions = new List<Point>();


    [FoldoutGroup("Collectable Item Generator")]
    public GameObject collectableItemPrefab;

    [FoldoutGroup("Collectable Item Generator")]
    [ListDrawerSettings(Expanded = false, DraggableItems = false)]
    public List<Point> collectableItemPositions = new List<Point>();


    public GlobalController globalCtrl;
    private EnemyManager enemyManager;
    private CollectableManager collectableManager;


    private void Awake()
    {
        this.enemyManager = this.globalCtrl.enemyManager;
        this.collectableManager = this.globalCtrl.collectableManager;
    }

    [FoldoutGroup("Enemy Generator")]
    [Button("Spawn Shadows", ButtonSizes.Medium)]
    public void SpawnShadows()
    {
        List<Tile> tiles = this.globalCtrl.sceneCtrl.tiles;

        foreach (Point point in this.enemyPositions)
        {
            Tile tile = tiles.Find(t => t.point == point);
            GameObject obj = GameObject.Instantiate(this.shadowPrefab, tile.obj.transform.position, Quaternion.identity);

            Enemy enemy = new Enemy("Enemy " + point, this.movementSpeed, obj, tile);
            this.enemyManager.enemies.Add(enemy);
        }
    }

    [FoldoutGroup("Collectable Item Generator")]
    [Button("Spawn Collectable Items", ButtonSizes.Medium)]
    public void SpawnCollectableItems()
    {
        foreach (Point point in this.collectableItemPositions)
        {
            this.collectableManager.SpawnItem(point, "Item " + point);
        }
    }

    [Button("Next Turn", ButtonSizes.Medium)]
    public void NextTurn()
    {
        this.globalCtrl.NextTurn();
    }
}