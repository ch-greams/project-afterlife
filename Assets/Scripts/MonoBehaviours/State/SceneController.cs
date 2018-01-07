using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class SceneController : SerializedMonoBehaviour
{
    public SceneType id;
    public GlobalController globalCtrl;
    public GlobalState globalState;
    public Player player;

    [InlineEditor]
    public SceneState sceneState;

    [DictionaryDrawerSettings(IsReadOnly = true)]
    public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        this.player.UpdatePlayer(this.tiles[this.globalState.currentPosition], this.globalState.currentVisibility);

        this.UpdateTiles(this.player.tile);
    }

    public void UpdateTiles(Tile tile)
    {
        List<Tile> visibleTiles = tile.GetTiles(this.player.visibleRange, TileState.Hidden);
        List<Point> playerLayer = visibleTiles.Select(vt => vt.point).ToList();

        Dictionary<Point, TileState> currentMap = this.sceneState.GetCurrentMap(playerLayer);

        foreach (KeyValuePair<Point, Tile> kvp in this.tiles)
        {
            TileData tileData = kvp.Value.obj.GetComponent<Interactable>().data as TileData;
            tileData.RefreshTileState(currentMap[kvp.Key], false);
        }
    }

#if UNITY_EDITOR

    [Button(ButtonSizes.Medium)]
    public void CopyDefaultTileStatesToMap()
    {
        this.sceneState.defaultMap = this.tiles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.state);
    }

#endif
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
