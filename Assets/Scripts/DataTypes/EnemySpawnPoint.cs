using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawnPoint : SerializedMonoBehaviour
{
    [FoldoutGroup("Enemy Main Settings", true)]
    public new string name = "Enemy";
    [FoldoutGroup("Enemy Main Settings", true)]
    public Point point;
    
    [FoldoutGroup("Enemy Settings")]
    public GameObject prefab;
    [FoldoutGroup("Enemy Settings")]
    public GameObject deathEffectPrefab;
    [FoldoutGroup("Enemy Settings")]
    public float movementSpeed = 3.0F;
    [FoldoutGroup("Enemy Settings")]
    public float animationSpeed = 2.0F;
    [FoldoutGroup("Enemy Settings")]
    public float attackPower = 1.0F;
    [FoldoutGroup("Enemy Settings")]
    public bool isLockedOnPlayer = false;
    [FoldoutGroup("Enemy Settings")]
    public float itemDropRate = 0.30F;

    [FoldoutGroup("SpawnPoint Settings")]
    public bool repeatSpawn = true;
    [FoldoutGroup("SpawnPoint Settings")]
    public int repeatSpawnDelay = 3;
    [FoldoutGroup("SpawnPoint Settings")]
    public int repeatSpawnTurnsLeft = 3;
    [FoldoutGroup("SpawnPoint Settings")]
    public int repeatSpawnCapacity = 3;


    public EnemySpawnPoint() { }


    public Enemy TrySpawnEnemy(List<Tile> tiles)
    {
        bool isSpawnTime = this.repeatSpawn && this.CheckTurnForSpawn();
        Tile tile = isSpawnTime ? tiles.Find(t => t.point == this.point) : null;

        bool isTileAvailable = ( (tile != null) && !tile.isBlocked && !tile.isBlockedByPlayer );
        bool isSpawnPointHasCharge = ( this.repeatSpawnCapacity > 0 );

        return ( (isTileAvailable && isSpawnPointHasCharge) ? this.SpawnEnemy(tile) : null );
    }

    private Enemy SpawnEnemy(Tile tile)
    {
        this.repeatSpawnCapacity--;

        return new Enemy(
            name: string.Format("{0} {1}", this.name, point),
            animationSpeed: this.animationSpeed,
            movementSpeed: this.movementSpeed,
            attackPower: this.attackPower,
            isLockedOnPlayer: this.isLockedOnPlayer,
            characterObject: GameObject.Instantiate(this.prefab, tile.gameObject.transform.position, Quaternion.identity),
            deathEffectPrefab: this.deathEffectPrefab,
            tile: tile
        );
    }

    private bool CheckTurnForSpawn()
    {
        bool isSpawnTime = !(this.repeatSpawnTurnsLeft > 0);

        this.repeatSpawnTurnsLeft = isSpawnTime ? this.repeatSpawnDelay : (this.repeatSpawnTurnsLeft - 1);

        return isSpawnTime;
    }

    [FoldoutGroup("Enemy Main Settings", true)]
    [Button(ButtonSizes.Medium)]
    public void RefreshCurrentPoint()
    {
        Point point = new Point(this.transform.position);

        this.point = point;
        this.transform.position = point.CalcWorldCoord(0.5F);
        this.transform.name = string.Format("spawn_{0} {1}", this.name, this.point);
    }
}
