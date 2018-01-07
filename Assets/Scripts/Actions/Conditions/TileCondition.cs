

public class TileCondition : ICondition
{
    public TileConditionType type = TileConditionType.IS_ACTIVE;
    private TileData tileData;


    public void Init(Interactable interactable)
    {
        this.tileData = interactable.data as TileData;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case TileConditionType.IS_ACTIVE:
                return (this.tileData.tile.state == TileState.Active);
            case TileConditionType.IS_HIDDEN:
                return (this.tileData.tile.state == TileState.Hidden);
            case TileConditionType.IS_DISABLED:
                return (this.tileData.tile.state == TileState.Disabled);
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
