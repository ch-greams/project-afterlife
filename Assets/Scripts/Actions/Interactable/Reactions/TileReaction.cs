using System.Collections;
using System.Collections.Generic;


public class TileReaction : IInteractableReaction
{
    public TileReactionType type;
    public List<Tile> tiles = new List<Tile>();

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case TileReactionType.BlockTiles:
                foreach (Tile tile in this.tiles)
                {
                    tile.RefreshTileState(tile.isVisible, true);
                }
                break;
            case TileReactionType.UnblockTiles:
                foreach (Tile tile in this.tiles)
                {
                    tile.RefreshTileState(tile.isVisible, false);
                }
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum TileReactionType
{
    BlockTiles,
    UnblockTiles,
}
