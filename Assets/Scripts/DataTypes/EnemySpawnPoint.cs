using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawnPoint : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    [FoldoutGroup("Enemy Settings", true)]
    public new string name = "Enemy";
    [FoldoutGroup("Enemy Settings", true)]
    public Point point;
    
    [FoldoutGroup("Enemy Settings/Parameters")]
    public GameObject prefab;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public GameObject deathEffectPrefab;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public float movementSpeed = 3.0F;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public float animationSpeed = 2.0F;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public float attackPower = 1.0F;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public bool isLockedOnPlayer = false;
    [FoldoutGroup("Enemy Settings/Parameters")]
    public float itemDropRate = 0.30F;

    [FoldoutGroup("SpawnPoint Settings")]
    public bool isSpawnPointActive = true;
    [FoldoutGroup("SpawnPoint Settings")]
    public GameObject spawnPointObject;
    [FoldoutGroup("SpawnPoint Settings")]
    public int spawnPointDelay = 3;
    [FoldoutGroup("SpawnPoint Settings")]
    public bool isSpawnPointCapacityInfinite = false;
    [FoldoutGroup("SpawnPoint Settings"), HideIf("isSpawnPointCapacityInfinite")]
    public int spawnPointCapacity = 3;
    [FoldoutGroup("SpawnPoint Settings")]
    public int turnsLeftTillNextSpawn = 3;


    private void OnDrawGizmos()
    {
        if (this.showGizmo)
        {
            Gizmos.DrawIcon(transform.position, "md-pin");
        }
    }


    public EnemySpawnPoint() { }


    public Enemy TrySpawnEnemy(List<Tile> tiles)
    {
        if (this.isSpawnPointActive)
        {
            Tile tile = this.CheckTurnForSpawn() ? tiles.Find(t => t.point == this.point) : null;

            bool isTileAvailable = ( (tile != null) && !tile.isBlocked && !tile.isBlockedByPlayer );
            bool isSpawnPointHasCharge = this.isSpawnPointCapacityInfinite || ( this.spawnPointCapacity > 0 );

            this.ToggleSpawnPoint(isSpawnPointHasCharge);

            return ( (isTileAvailable && isSpawnPointHasCharge) ? this.SpawnEnemy(tile) : null );
        }

        return null;
    }

    [FoldoutGroup("SpawnPoint Settings"), Button(ButtonSizes.Medium)]
    public void ToggleSpawnPoint()
    {
        this.ToggleSpawnPoint(!this.isSpawnPointActive);
    }

    public void ToggleSpawnPoint(bool enable)
    {
        this.isSpawnPointActive = enable;

        if (this.spawnPointObject)
        {
            this.spawnPointObject.SetActive(this.isSpawnPointActive);
        }
    }

    private Enemy SpawnEnemy(Tile tile)
    {
        if (!this.isSpawnPointCapacityInfinite)
        {
            this.spawnPointCapacity--;
        }

        return new Enemy(
            name: string.Format("{0} {1}", this.name, point),
            animationSpeed: this.animationSpeed,
            movementSpeed: this.movementSpeed,
            attackPower: this.attackPower,
            isLockedOnPlayer: this.isLockedOnPlayer,
            itemDropRate: this.itemDropRate,
            characterObject: GameObject.Instantiate(this.prefab, tile.gameObject.transform.position, Quaternion.identity),
            deathEffectPrefab: this.deathEffectPrefab,
            tile: tile
        );
    }

    private bool CheckTurnForSpawn()
    {
        bool isSpawnTime = !(this.turnsLeftTillNextSpawn > 0);

        this.turnsLeftTillNextSpawn = isSpawnTime ? this.spawnPointDelay : (this.turnsLeftTillNextSpawn - 1);

        return isSpawnTime;
    }



    [FoldoutGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh")]
    public SceneController sceneCtrl;

    [FoldoutGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh"), ShowInInspector]
    private Vector3 offset { get { return (
        (this.sceneCtrl != null)
            ? new Vector3(x: this.sceneCtrl.transform.position.x, y: 0.5F, z: this.sceneCtrl.transform.position.z)
            : Vector3.zero
    ); } }

    [FoldoutGroup("Enemy Settings", true), BoxGroup("Enemy Settings/Current Point Refresh"), Button(ButtonSizes.Medium)]
    private void RefreshCurrentPoint()
    {
        this.point = new Point(this.transform.position - this.offset);
        this.transform.position = this.point.CalcWorldCoord(new Vector3(
            x: this.offset.x + 0.5F, y: this.offset.y, z: this.offset.z + 0.5F
        ));
        this.transform.name = string.Format("spawn_{0} {1}", this.name, this.point);
    }
}
