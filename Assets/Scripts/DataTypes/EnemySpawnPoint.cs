using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawnPoint : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    [BoxGroup("Enemy Settings", true)]
    public Enemy enemy;

    [BoxGroup("Enemy Settings/State", true), InlineProperty, HideLabel]
    public EnemyState enemyState;

    [BoxGroup("SpawnPoint Settings")]
    public GameObject spawnPointObject;
    [BoxGroup("SpawnPoint Settings"), BoxGroup("SpawnPoint Settings/State"), InlineProperty, HideLabel]
    public EnemySpawnPointState state;


    private void OnDrawGizmos()
    {
        if (this.showGizmo)
        {
            Gizmos.DrawIcon(transform.position, "md-pin");
        }
    }


    public EnemySpawnPoint() { }


    public Enemy TrySpawnEnemy(List<Tile> tiles, SceneController sceneCtrl)
    {
        if (this.state.isActive)
        {
            Tile tile = this.CheckTurnForSpawn() ? tiles.Find(t => t.point == this.enemyState.position) : null;

            bool isTileAvailable = ( (tile != null) && !tile.isBlocked && !tile.isBlockedByPlayer );
            bool isSpawnPointHasCharge = this.state.hasInfiniteCapacity || ( this.state.capacity > 0 );

            this.ToggleSpawnPoint(isSpawnPointHasCharge);

            return ( (isTileAvailable && isSpawnPointHasCharge) ? this.SpawnEnemy(tile, sceneCtrl) : null );
        }

        return null;
    }

    [BoxGroup("SpawnPoint Settings"), Button(ButtonSizes.Medium)]
    public void ToggleSpawnPoint()
    {
        this.ToggleSpawnPoint(!this.state.isActive);
    }

    public void ToggleSpawnPoint(bool enable)
    {
        this.state.isActive = enable;

        if (this.spawnPointObject)
        {
            this.spawnPointObject.SetActive(this.state.isActive);
        }
    }

    private Enemy SpawnEnemy(Tile tile, SceneController sceneCtrl)
    {
        if (!this.state.hasInfiniteCapacity)
        {
            this.state.capacity--;
        }

        this.state.spawnCount++;

        EnemyState enemyState = new EnemyState(
            id: string.Format("{0}_{1:D3}", this.enemyState.type, this.state.spawnCount),
            position: this.enemyState.position,
            enemyState: this.enemyState
        );

        Enemy enemy = Enemy.Instantiate(
            original: this.enemy,
            tile: tile,
            enemyState: enemyState
        );

        enemy.InitActions(sceneCtrl.globalCtrl);

        return enemy;
    }

    private bool CheckTurnForSpawn()
    {
        bool isSpawnTime = !(this.state.nextSpawn > 0);

        this.state.nextSpawn = isSpawnTime ? this.state.spawnDelay : (this.state.nextSpawn - 1);

        return isSpawnTime;
    }



    [BoxGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh")]
    public SceneController sceneCtrl;

    [BoxGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh"), ShowInInspector]
    private Vector3 offset { get { return (
        (this.sceneCtrl != null)
            ? new Vector3(x: this.sceneCtrl.transform.position.x, y: 0.5F, z: this.sceneCtrl.transform.position.z)
            : Vector3.zero
    ); } }

    [BoxGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh"), Button(ButtonSizes.Medium)]
    private void RefreshCurrentPoint()
    {
        this.enemyState.position = new Point(this.transform.position - this.offset);
        this.transform.position = this.enemyState.position.CalcWorldCoord(this.offset, this.offset.y, 0.5F);

        this.transform.name = string.Format("spawn_{0} {1}", this.enemyState.type, this.enemyState.position);
    }
}


[Serializable, InlineProperty]
public class EnemySpawnPointState
{
    public bool isActive = true;

    public int spawnCount = 0;

    [HorizontalGroup("Spawn", 0.5F)]
    public int nextSpawn = 3;
    [HorizontalGroup("Spawn", 0.5F)]
    public int spawnDelay = 3;


    [HorizontalGroup("Capacity", 0.5F)]
    public bool hasInfiniteCapacity = false;
    [HorizontalGroup("Capacity", 0.5F), HideIf("hasInfiniteCapacity", false)]
    public int capacity = 3;


    public EnemySpawnPointState(EnemySpawnPointState esps)
    {
        this.isActive = esps.isActive;
        
        this.spawnCount = esps.spawnCount;

        this.nextSpawn = esps.nextSpawn;
        this.spawnDelay = esps.spawnDelay;

        this.hasInfiniteCapacity = esps.hasInfiniteCapacity;
        this.capacity = esps.capacity;
    }
}
