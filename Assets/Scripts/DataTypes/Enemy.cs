using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class Enemy : SerializedMonoBehaviour
{
    [BoxGroup("State"), InlineProperty, HideLabel]
    public EnemyState state;

    public float animationSpeed = 6.0F;
    public GameObject characterObject;
    public GameObject deathEffectPrefab;

    public Tile tile;

    public List<EnemyAction> spawnActions = new List<EnemyAction>();
    public List<EnemyAction> deathActions = new List<EnemyAction>();


    public Enemy(string name, float movementSpeed, GameObject characterObject, Tile tile)
    {
        this.state = new EnemyState(
            id: name,
            type: "default",
            position: tile.point,
            movementSpeed: movementSpeed,
            attackPower: 1.0F,
            itemDropRate: 0.3F,
            isLockedOn: true
        );

        this.characterObject = characterObject;
        this.characterObject.name = this.state.id;

        this.tile = tile;
        this.tile.RefreshTileState(this.tile.isVisible, true);
    }

    public static Enemy Instantiate(Enemy original, Tile tile, EnemyState enemyState)
    {
        Enemy enemy = GameObject.Instantiate(
            original: original, 
            position: tile.gameObject.transform.position,
            rotation: Quaternion.identity
        );

        enemy.state = enemyState;
        enemy.characterObject.name = enemy.state.id;

        enemy.tile = tile;
        enemy.tile.RefreshTileState(tile.isVisible, true);

        return enemy;
    }

    public void InitActions(GlobalController globalCtrl)
    {
        this.spawnActions.ForEach(action => action.Init(globalCtrl));
        this.deathActions.ForEach(action => action.Init(globalCtrl));
    }

    public IEnumerator OnSpawn()
    {
        yield return this.TriggerValidActions(this.spawnActions);
    }

    public IEnumerator OnDeath()
    {
        yield return this.TriggerValidActions(this.deathActions);
    }

    private IEnumerator TriggerValidActions(List<EnemyAction> actions)
    {
        foreach (EnemyAction action in actions)
        {
            if (action.IsValid())
            {
                yield return action.React();
            }
        }
    }

    public IEnumerator MoveToPlayer(Player player, EnemyManager enemyManager)
    {
        Path<Tile> path = player.tile.FindPathFrom(this.tile, (t) => (!t.isBlocked), this.state.movementSpeed);
        
        if (path != null)
        {
            yield return this.MoveOnPath(path.Reverse(), player, enemyManager);
        }
    }

    private IEnumerator TryAttackPlayer(Player player, EnemyManager enemyManager)
    {
        if (this.tile.isBlockedByPlayer)
        {
            // Attack player
            player.ChangeVisibleRange(player.visibleRange - this.state.attackPower);

            // Destroy enemy
            yield return enemyManager.TryDestroyEnemyOnPoint(this.tile.point, false);
        }
    }

    public IEnumerator Destroy()
    {
        yield return this.OnDeath();

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
            yield return this.TryAttackPlayer(player, enemyManager);
        }
    }

    private IEnumerator MoveToTile(Tile tile)
    {
        float startTime = Time.time;
        Transform characterTransform = this.characterObject.transform;
        Vector3 startPosition = characterTransform.position;
        Vector3 endPosition = new Vector3(tile.gameObject.transform.position.x, 0, tile.gameObject.transform.position.z);
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

        this.state.position = tile.point;
    }
}


[Serializable, InlineProperty]
public class EnemyState
{
    public string id;
    public string type;

    public float movementSpeed;
    public float attackPower;
    public float itemDropRate;

    public Point position;
    public bool isLockedOn = false;


    public EnemyState(
        string id, 
        string type, 
        Point position, 
        float movementSpeed, 
        float attackPower, 
        float itemDropRate, 
        bool isLockedOn
    ) {
        this.id = id;
        this.type = type;
        this.position = position;

        this.movementSpeed = movementSpeed; 
        this.attackPower = attackPower; 
        this.itemDropRate = itemDropRate; 
        
        this.isLockedOn = isLockedOn;
    }

    public EnemyState(
        string id, 
        Point position, 
        EnemyState enemyState
    ) {
        this.id = id;
        this.position = position;

        this.type = enemyState.type;
        this.movementSpeed = enemyState.movementSpeed; 
        this.attackPower = enemyState.attackPower; 
        this.itemDropRate = enemyState.itemDropRate; 
        this.isLockedOn = enemyState.isLockedOn;
    }
}
