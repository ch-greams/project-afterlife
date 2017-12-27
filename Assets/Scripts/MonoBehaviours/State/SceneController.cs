using System.Collections.Generic;
using Sirenix.OdinInspector;


public class SceneController : SerializedMonoBehaviour
{
    public GlobalController globalCtrl;
    public GlobalState globalState;
    public Player player;
    [InlineEditor]
    public List<Tile> tiles = new List<Tile>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalState = this.globalCtrl.globalState;

        this.MovePlayerToStartPoint();
    }

    private void MovePlayerToStartPoint()
    {
        Tile tile = this.tiles.Find(t => t.point == this.globalState.currentPosition);
        this.player.tile = tile;
        this.player.playerTransform.position = tile.obj.transform.position;
    }
}

public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
    Hallway,
    AptN3_LivingRoom,
    AptN0_LivingRoom,
    AptN1_Bathroom,
    AptN3_Bathroom,
    AptN3_Bedroom,
    AptN5_Bathroom,
    AptN5_Bedroom,
    AptN5_LivingRoom,
}
