

public class TileCondition : ICondition
{
    private bool passable;

    public void Init(Interactable interactable)
    {
        this.passable = (interactable.data as TileData).tile.passable;
    }

    public bool IsValid()
    {
        return this.passable;
    }
}
