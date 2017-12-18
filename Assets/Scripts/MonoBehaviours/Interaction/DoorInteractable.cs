using System.Collections.Generic;
using Sirenix.OdinInspector;


public class DoorInteractable : Interactable
{
    [Required]
    public Door door;

    public List<Tile> attachedTiles = new List<Tile>();

}
