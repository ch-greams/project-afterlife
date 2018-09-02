using System.Collections;
using System.Linq;
using UnityEngine;


public class Enemy
{
    public string name;
    public float speed = 6.0F;

    public Transform characterTransform;
    public Tile tile;


    public Enemy() { }

    public Enemy(string name, Transform characterTransform, Tile tile)
    {
        this.name = name;
        this.characterTransform = characterTransform;
        this.tile = tile;
    }

    public IEnumerator MoveToPlayer(Player player)
    {
        Path<Tile> path = player.tile.FindPathFrom(this.tile, false);
        Tile nextTile = path.Reverse().ElementAt(1);

        Debug.Log("nextTile - " + nextTile);

        yield return this.MoveToTile(nextTile);
    }


    public IEnumerator MoveToTile(Tile tile)
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

        this.tile.isBlocked = false;
        this.tile = tile;
        this.tile.isBlocked = true;
    }
}
