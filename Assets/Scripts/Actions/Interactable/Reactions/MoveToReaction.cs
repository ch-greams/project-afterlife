using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MoveToReaction : IInteractableReaction
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

    // TODO: Check if movement logic is necessary later
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

            Tile tile = this.GetClosestTile();

            // this.textureAnimator.Play();
            Path<Tile> path = tile.FindPathFrom(player.tile, (t) => (!t.isBlocked && t.isVisible));
            if (path != null)
            {
                yield return this.MoveOnPath(player, path.Reverse());
            }

            if (!this.sceneCtrl.sceneState.visibleByDefault)
            {
                this.sceneCtrl.UpdateTiles(tile);
            }

            player.isMoving = false;
        }
    }

    private Tile GetClosestTile()
    {
        Point curPoint = this.sceneCtrl.player.tile.point;

        this.tiles.Sort((ti1, ti2) =>
        {
            float est1 = ti1.point.EstimateTo(curPoint);
            float est2 = ti2.point.EstimateTo(curPoint);
            return est1.CompareTo(est2);
        });

        return this.tiles.First();
    }

    private IEnumerator MoveOnPath(Player player, IEnumerable<Tile> path)
    {
        foreach (Tile tile in path)
        {
            player.characterAnimator.SetFloat(this.speedParamHash, player.animationSpeed * 1F);

            float startTime = Time.time;
            Vector3 startPosition = player.playerTransform.position;
            Vector3 endPosition = new Vector3(tile.obj.transform.position.x, 0, tile.obj.transform.position.z);
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            player.characterTransform.LookAt(endPosition);

            while (endPosition != player.playerTransform.position)
            {
                float distCovered = (Time.time - startTime) * player.animationSpeed;
                float fracJourney = distCovered / journeyLength;
                player.playerTransform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.sceneCtrl.globalCtrl.UpdatePlayerPosition(tile.point);

            player.tile.isBlockedByPlayer = false;
            player.tile = tile;
            player.tile.isBlockedByPlayer = true;

            player.characterAnimator.SetFloat(this.speedParamHash, 0F);

            if (player.isTargetUpdating)
            {
                break;
            }
        }
    }
}
