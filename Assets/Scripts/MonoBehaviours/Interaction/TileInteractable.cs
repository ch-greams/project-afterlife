using System.Collections;
using System.Linq;
using UnityEngine;


public class TileInteractable : Interactable
{
    public Tile tile;
    public PlayerControl playerControl;
    private AnimateTiledTexture textureAnimator;


    private void Start()
    {
        this.textureAnimator = this.GetComponent<AnimateTiledTexture>();
    }

    protected override IEnumerator OnLeftClick()
    {
        if (tile.passable)
        {
            yield return this.MoveToThisTile();
        }
    }

    public IEnumerator MoveToThisTile()
    {
        this.textureAnimator.Play();
        yield return this.playerControl.MoveToTileAsync(this.tile);
    }
}
