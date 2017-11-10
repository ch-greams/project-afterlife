using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Grid : MonoBehaviour
{
    public List<TileInteractable> tileInteractables = new List<TileInteractable>();

    // TODO: Remove later
    public void Awake()
    {
        List<Tile> tiles = tileInteractables.Select(ti => ti.tile).ToList();
        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }
}
