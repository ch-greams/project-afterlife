using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneStateController : MonoBehaviour
{
    public SceneType type;
    public GlobalStateController globalCtrl;
    public GlobalState globalState;
    public SceneState sceneState;
    public SceneController sceneController;
    public PlayerController playerControl;
    [InlineEditor]
    public List<Tile> tiles = new List<Tile>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalStateController>();
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.type];
        this.sceneController = FindObjectOfType<SceneController>();

        // Move Player to startPoint
        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        Tile tile = this.tiles.Find(t => t.point == this.sceneState.position);
        this.playerControl.currentTile = tile;
        this.playerControl.transform.position = tile.obj.transform.position;
    }
}
