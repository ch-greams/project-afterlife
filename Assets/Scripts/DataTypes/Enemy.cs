using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Enemy
{
    public string name;
    public float speed = 6.0F;
    public float visibleRange = 3.0F;
    public bool isLockedOnPlayer = false;

    public Transform characterTransform;
    public Tile tile;


    public Enemy() { }

    public Enemy(string name, float visibleRange, Transform characterTransform, Tile tile)
    {
        this.name = name;
        this.visibleRange = visibleRange;

        this.characterTransform = characterTransform;
        this.tile = tile;

        this.characterTransform.gameObject.name = this.name;
        this.tile.isBlocked = true;
    }

    public IEnumerator MoveToPlayer(Player player)
    {
        Path<Tile> path = player.tile.FindPathFrom(this.tile, (t) => (!t.isBlocked), this.visibleRange);
        yield return this.MoveOnPath(path.Reverse());
    }


    private IEnumerator MoveOnPath(IEnumerable<Tile> path)
    {
        foreach (Tile tile in path)
        {
            float startTime = Time.time;
            Vector3 startPosition = this.characterTransform.position;
            Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            this.characterTransform.LookAt(endPosition);

            while (endPosition != this.characterTransform.position)
            {
                float distCovered = (Time.time - startTime) * this.speed;
                float fracJourney = distCovered / journeyLength;
                this.characterTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.tile.RefreshTileState(this.tile.isVisible, false);
            this.tile = tile;
            this.tile.RefreshTileState(this.tile.isVisible, true);
        }
    }
}
