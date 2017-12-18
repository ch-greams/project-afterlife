using Sirenix.OdinInspector;


public class TileInteractable : Interactable
{
    [BoxGroup("Tile Editor")]
    [InlineEditor(Expanded = true)]
    public Tile tile;


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
}
