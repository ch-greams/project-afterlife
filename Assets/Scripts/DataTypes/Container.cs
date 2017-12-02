using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class Container
{
    [Required]
    public SceneController sceneCtrl;

    public ContainerType type;

    public Container() { }

    public void AddItemsToInventory()
    {
        this.sceneCtrl.globalCtrl.inventory.AddItems(this.GetItems());
    }

    public List<Item> GetItems()
    {
        SceneState sceneState = this.sceneCtrl.sceneState;

        List<Item> items = sceneState.containers[type];
        sceneState.containers[type] = new List<Item>();
        return items;
    }

    public bool IsNotEmpty()
    {
        return this.sceneCtrl.sceneState.containers[type].Any();
    }
}


public enum ContainerType
{
    AptN1_Bedroom_Table,
    AptN1_Bedroom_Bed,
}
