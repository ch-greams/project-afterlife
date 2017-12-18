using System.Collections.Generic;
using Sirenix.OdinInspector;


public class ContainerInteractable : Interactable
{
    [Required]
    public Container container;

    public List<Tile> attachedTiles = new List<Tile>();

}
