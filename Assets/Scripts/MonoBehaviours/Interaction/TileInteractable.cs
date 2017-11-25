using System.Collections;
using Sirenix.OdinInspector;


public class TileInteractable : Interactable
{
    [BoxGroup("Tile Editor")]
    [InlineEditor(Expanded = true)]
    public Tile tile;
    public PlayerController playerControl;
    private AnimateTiledTexture textureAnimator;

#if UNITY_EDITOR

    [BoxGroup("Tile Editor")]
    [ButtonGroup("Tile Editor/Passable")]
    [Button("Make Possible", ButtonSizes.Medium)]
    public void MakePossible()
    {
        this.tile.passable = true;
    }

    [BoxGroup("Tile Editor")]
    [ButtonGroup("Tile Editor/Passable")]
    [Button("Make Impossible", ButtonSizes.Medium)]
    public void MakeImpossible()
    {
        this.tile.passable = false;
    }

#endif

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

    // TODO: Optimize this call for Tile class
    public IEnumerator MoveToThisTile()
    {
        this.textureAnimator.Play();
        yield return this.playerControl.MoveToTile(this.tile);
    }
}
