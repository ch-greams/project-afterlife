using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class MoveToReaction : IReaction
{
    private List<Tile> tiles = new List<Tile>();
    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;

        switch (interactable.GetType().Name)
        {
            case "ContainerInteractable":
                this.tiles = (interactable as ContainerInteractable).attachedTiles;
                break;
            case "DoorInteractable":
                this.tiles = (interactable as DoorInteractable).attachedTiles;
                break;
            default:
                break;
        }
    }
    public IEnumerator React()
    {
        if (this.tiles.Any())
        {
            Point curPoint = this.sceneCtrl.playerCtrl.currentTile.point;

            this.tiles.Sort((ti1, ti2) =>
            {
                double est1 = ti1.point.EstimateTo(curPoint);
                double est2 = ti2.point.EstimateTo(curPoint);
                return est1.CompareTo(est2);
            });

            yield return this.tiles.First().obj.GetComponent<TileInteractable>().MoveToThisTile();
        }
    }
}
