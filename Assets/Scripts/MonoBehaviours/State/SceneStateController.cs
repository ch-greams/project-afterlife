using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneStateController : MonoBehaviour
{
    public SceneType type;
    public GlobalStateController globalState;
    public SceneController sceneController;
    public PlayerController playerControl;
    [InlineEditor]
    public List<Tile> tiles = new List<Tile>();


    private void Awake()
    {
        this.globalState = FindObjectOfType<GlobalStateController>();
        this.sceneController = FindObjectOfType<SceneController>();

        // Move Player to startPoint
        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        Tile tile = this.tiles.Find(t =>
        {
            return t.point == this.globalState.positionInScene[this.type];
        });
        this.playerControl.currentTile = tile;
        this.playerControl.transform.position = tile.obj.transform.position;
    }
}
