using Sirenix.OdinInspector;
﻿using System.Collections.Generic;
using UnityEngine;


public class EnemySpawnPoint
{    
    [FoldoutGroup("Enemy Main Settings", true)]
    public Point point;
    [FoldoutGroup("Enemy Main Settings", true)]
    public string name = "Enemy";
    [FoldoutGroup("Enemy Settings")]
    public GameObject prefab;
    [FoldoutGroup("Enemy Settings")]
    public float movementSpeed = 3.0F;
    [FoldoutGroup("Enemy Settings")]
    public float animationSpeed = 2.0F;
    [FoldoutGroup("Enemy Settings")]
    public float attackPower = 1.0F;
    [FoldoutGroup("Enemy Settings")]
    public bool isLockedOnPlayer = false;

    [FoldoutGroup("SpawnPoint Settings")]
    public bool repeatSpawn = true;
    [FoldoutGroup("SpawnPoint Settings")]
    public int repeatSpawnDelay = 3;
    [FoldoutGroup("SpawnPoint Settings")]
    public int repeatSpawnTurnsLeft = 3;


    public EnemySpawnPoint() { }


    public Enemy TrySpawnEnemy(List<Tile> tiles)
    {
        bool isSpawnTime = this.repeatSpawn && this.CheckTurnForSpawn();
        Tile tile = isSpawnTime ? tiles.Find(t => t.point == this.point) : null;

        return ((tile != null && !tile.isBlocked && !tile.isBlockedByPlayer) ? this.SpawnEnemy(tile) : null);
    }

    private Enemy SpawnEnemy(Tile tile)
    {
        GameObject obj = GameObject.Instantiate(this.prefab, tile.obj.transform.position, Quaternion.identity);
        string enemyName = string.Format("{0} {1}", this.name, point);

        return new Enemy(enemyName, this.movementSpeed, this.attackPower, this.isLockedOnPlayer, obj.transform, tile);
    }

    private bool CheckTurnForSpawn()
    {
        if (this.repeatSpawnTurnsLeft > 0)
        {
            this.repeatSpawnTurnsLeft = this.repeatSpawnTurnsLeft - 1;
            return false;
        }
        else
        {
            this.repeatSpawnTurnsLeft = this.repeatSpawnDelay;
            return true;
        }
    }
}
