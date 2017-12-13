using System.Collections.Generic;
using Sirenix.OdinInspector;


public class SceneController : SerializedMonoBehaviour
{
    public GlobalController globalCtrl;
    public GlobalState globalState;
    public SceneState sceneState;
    public PlayerController playerCtrl;
    public Scene scene;
    [InlineEditor]
    public List<Tile> tiles = new List<Tile>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.scene.type];

        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        Tile tile = this.tiles.Find(t => t.point == this.sceneState.position);
        this.playerCtrl.currentTile = tile;
        this.playerCtrl.transform.position = tile.obj.transform.position;
    }
}
