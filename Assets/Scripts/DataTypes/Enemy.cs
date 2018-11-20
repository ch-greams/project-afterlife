using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Enemy
{
    public string name;
    public float animationSpeed = 6.0F;
    public float movementSpeed = 3.0F;
    public float attackPower = 1.0F;
    public bool isLockedOnPlayer = false;
    public float itemDropRate = 0.30F;

    public GameObject characterObject;
    public GameObject deathEffectPrefab;
    public Tile tile;


    public Enemy() { }

    public Enemy(string name, float movementSpeed, GameObject characterObject, Tile tile)
    {
        this.name = name;
        this.movementSpeed = movementSpeed;

        this.characterObject = characterObject;
        this.characterObject.name = this.name;

        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }

    public Enemy(
        string name,
        float animationSpeed,
        float movementSpeed,
        float attackPower,
        bool isLockedOnPlayer,
        GameObject characterObject,
        GameObject deathEffectPrefab,
        Tile tile
    ) {
        this.name = name;
        this.animationSpeed = animationSpeed;

        this.movementSpeed = movementSpeed;
        this.attackPower = attackPower;
        this.isLockedOnPlayer = isLockedOnPlayer;

        this.characterObject = characterObject;
        this.characterObject.name = this.name;

        this.deathEffectPrefab = deathEffectPrefab;

        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }

    public IEnumerator MoveToPlayer(Player player, EnemyManager enemyManager)
    {
        Path<Tile> path = player.tile.FindPathFrom(this.tile, (t) => (!t.isBlocked), this.movementSpeed);
        
        if (path != null)
        {
            yield return this.MoveOnPath(path.Reverse(), player, enemyManager);
        }
    }

    private bool TryAttackPlayer(Player player, EnemyManager enemyManager)
    {
        if (this.tile.isBlockedByPlayer)
        {
            // Attack player
            player.ChangeVisibleRange(player.visibleRange - this.attackPower);

            // Destroy enemy
            return enemyManager.TryDestroyEnemyOnPoint(this.tile.point, false);
        }

        return false;
    }

    public void Destroy()
    {
        Vector3 effectPosition = this.characterObject.transform.position + new Vector3(0, 0.5F, 0);
        GameObject.Instantiate(this.deathEffectPrefab, effectPosition, Quaternion.identity);

        GameObject.DestroyImmediate(this.characterObject);
        this.tile.RefreshTileState(this.tile.isVisible, false);
    }

    private IEnumerator MoveOnPath(IEnumerable<Tile> path, Player player, EnemyManager enemyManager)
    {
        foreach (Tile tile in path)
        {
            yield return this.MoveToTile(tile);

            // TODO: Make this shit bearable
            this.TryAttackPlayer(player, enemyManager);
        }
    }

    private IEnumerator MoveToTile(Tile tile)
    {
        float startTime = Time.time;
        Transform characterTransform = this.characterObject.transform;
        Vector3 startPosition = characterTransform.position;
        Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
        float journeyLength = Vector3.Distance(startPosition, endPosition);

        characterTransform.LookAt(endPosition);

        while (endPosition != characterTransform.position)
        {
            float distCovered = (Time.time - startTime) * this.animationSpeed;
            float fracJourney = distCovered / journeyLength;
            characterTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

            yield return null;
        }

        this.tile.RefreshTileState(this.tile.isVisible, false);
        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }
}
