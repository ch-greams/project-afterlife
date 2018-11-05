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

    public Transform characterTransform;
    public Tile tile;


    public Enemy() { }

    public Enemy(string name, float movementSpeed, Transform characterTransform, Tile tile)
    {
        this.name = name;
        this.movementSpeed = movementSpeed;

        this.characterTransform = characterTransform;
        this.characterTransform.gameObject.name = this.name;

        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }

    public Enemy(string name, float movementSpeed, float attackPower, bool isLockedOnPlayer, Transform characterTransform, Tile tile)
    {
        this.name = name;

        this.movementSpeed = movementSpeed;
        this.attackPower = attackPower;
        this.isLockedOnPlayer = isLockedOnPlayer;

        this.characterTransform = characterTransform;
        this.characterTransform.gameObject.name = this.name;

        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }

    public IEnumerator MoveToPlayer(Player player, EnemyManager enemyManager)
    {
        Path<Tile> path = player.tile.FindPathFrom(this.tile, (t) => (!t.isBlocked), this.movementSpeed);
        yield return this.MoveOnPath(path.Reverse(), player, enemyManager);
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
        GameObject.DestroyImmediate(this.characterTransform.gameObject);
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
        Vector3 startPosition = this.characterTransform.position;
        Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
        float journeyLength = Vector3.Distance(startPosition, endPosition);

        this.characterTransform.LookAt(endPosition);

        while (endPosition != this.characterTransform.position)
        {
            float distCovered = (Time.time - startTime) * this.animationSpeed;
            float fracJourney = distCovered / journeyLength;
            this.characterTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

            yield return null;
        }

        this.tile.RefreshTileState(this.tile.isVisible, false);
        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }
}
