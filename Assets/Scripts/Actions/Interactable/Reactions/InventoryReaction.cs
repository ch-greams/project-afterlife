using System.Collections;


public class InventoryReaction : IInteractableReaction
{
    public InventoryReactionType type;

    public Item item;

    private GlobalController globalCtrl;


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case InventoryReactionType.ADD_ITEM:
                this.globalCtrl.inventory.AddItem(this.item);
                break;
            case InventoryReactionType.REMOVE_ITEM:
                this.globalCtrl.inventory.RemoveItem(this.item);
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum InventoryReactionType
{
    ADD_ITEM,
    REMOVE_ITEM,
}