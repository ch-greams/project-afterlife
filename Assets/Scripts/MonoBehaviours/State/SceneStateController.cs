using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SceneStateController : MonoBehaviour
{
    public SceneType type;
    public GlobalStateController globalState;
    public SceneController sceneController;
    public PlayerController playerControl;

    public List<TileInteractable> tileInteractables = new List<TileInteractable>();


    private void Awake()
    {
        this.globalState = FindObjectOfType<GlobalStateController>();
        this.sceneController = FindObjectOfType<SceneController>();

        // TODO: temporary solution, mb replace with map?
        this.SetNeighboursForTiles();

        // Move Player to startPoint
        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        TileInteractable tile = this.tileInteractables.Find(ti =>
        {
            return ti.tile.point == this.globalState.positionInScene[this.type];
        });
        this.playerControl.currentTile = tile;
        this.playerControl.transform.position = tile.transform.position;
    }

    private void SetNeighboursForTiles()
    {
        List<Tile> tiles = tileInteractables.Select(ti => ti.tile).ToList();
        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }
}
