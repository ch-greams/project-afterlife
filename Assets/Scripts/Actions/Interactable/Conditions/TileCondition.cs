

public class TileCondition : IInteractableCondition
{
    public TileConditionType type = TileConditionType.IS_ACTIVE;
    private TileData tileData;


    public void Init(Interactable interactable)
    {
        this.tileData = interactable.data as TileData;
    }

    public bool IsValid()
    {
        Tile tile = this.tileData.tile;

        switch (this.type)
        {
            case TileConditionType.IS_ACTIVE:
                return (tile.isVisible && !tile.isBlocked);
            case TileConditionType.IS_HIDDEN:
                return !tile.isVisible;
            case TileConditionType.IS_DISABLED:
                return tile.isBlocked;
            default:
                return false;
        }
    }
}

public enum TileConditionType
{
    IS_ACTIVE,
    IS_HIDDEN,
    IS_DISABLED,
}
