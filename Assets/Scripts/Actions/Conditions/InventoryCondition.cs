

public class InventoryCondition : ICondition
{
    public InventoryConditionType type;
    
    public Item item;

    private GlobalState globalState;


    public void Init(Interactable interactable)
    {
        this.globalState = interactable.sceneCtrl.globalState;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case InventoryConditionType.ITEM_EXISTS:
                return this.globalState.inventory.Contains(this.item);
            case InventoryConditionType.ITEM_NOT_EXISTS:
                return !this.globalState.inventory.Contains(this.item);
            default:
                return false;
        }
    }
}

public enum InventoryConditionType
{
    ITEM_EXISTS,
    ITEM_NOT_EXISTS,
}
