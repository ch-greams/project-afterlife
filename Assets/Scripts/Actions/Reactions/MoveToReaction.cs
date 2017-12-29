using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MoveToReaction : IReaction
{
    private List<Tile> tiles = new List<Tile>();
    private SceneController sceneCtrl;

    private int speedParamHash;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
        this.speedParamHash = Animator.StringToHash("Speed");

        switch (interactable.data.GetType().Name)
        {
            case "ContainerData":
            case "DoorData":
                this.tiles = interactable.data.neighbourTiles;
                break;
            case "TileData":
                this.tiles = new List<Tile>(){ (interactable.data as TileData).tile };
                break;
            default:
                break;
        }
    }

    public IEnumerator React()
    {
        if (this.tiles.Any())
        {
            Player player = this.sceneCtrl.player;

            while (player.isMoving)
            {
                player.isTargetUpdating = true;
                yield return null;
            }

            player.isTargetUpdating = false;
            player.isMoving = true;

            // TODO: Check how often this triggered
            Tile tile = this.GetClosestTile();

            // this.textureAnimator.Play();
            yield return this.MoveToTile(player, tile);

            player.isMoving = false;
        }
    }

    private Tile GetClosestTile()
    {
        Point curPoint = this.sceneCtrl.player.tile.point;

        this.tiles.Sort((ti1, ti2) =>
        {
            double est1 = ti1.point.EstimateTo(curPoint);
            double est2 = ti2.point.EstimateTo(curPoint);
            return est1.CompareTo(est2);
        });

        return this.tiles.First();
    }

    private IEnumerator MoveToTile(Player player, Tile targetTile)
    {
        foreach (Tile tile in targetTile.FindPathFrom(player.tile, true).Reverse())
        {
            player.characterAnimator.SetFloat(this.speedParamHash, player.speed * 1F);

            float startTime = Time.time;
            Vector3 startPosition = player.playerTransform.position;
            Vector3 endPosition = tile.obj.transform.position;
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            player.characterTransform.LookAt(endPosition);

            while (endPosition != player.playerTransform.position)
            {
                float distCovered = (Time.time - startTime) * player.speed;
                float fracJourney = distCovered / journeyLength;
                player.playerTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.sceneCtrl.globalCtrl.UpdatePlayerPosition(tile.point);

            player.tile = tile;
            player.characterAnimator.SetFloat(this.speedParamHash, 0F);

            if (player.isTargetUpdating)
            {
                break;
            }
        }
    }
}
