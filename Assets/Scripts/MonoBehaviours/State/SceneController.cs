using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneController : MonoBehaviour
{
    public SceneType type;
    public GlobalController globalCtrl;
    public GlobalState globalState;
    public SceneState sceneState;
    public PlayerController playerControl;
    [InlineEditor]
    public List<Tile> tiles = new List<Tile>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.type];

        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        Tile tile = this.tiles.Find(t => t.point == this.sceneState.position);
        this.playerControl.currentTile = tile;
        this.playerControl.transform.position = tile.obj.transform.position;
    }
}
