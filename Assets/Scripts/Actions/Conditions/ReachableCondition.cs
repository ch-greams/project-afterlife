

public class ReachableCondition : ICondition
{
    private SceneController sceneCtrl;
    private IDataInteractable data;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
        this.data = interactable.data;
    }

    public bool IsValid()
    {
        return this.data.neighbourTiles.Exists(
            tile => (tile.FindPathFrom(this.sceneCtrl.player.tile, TileState.Active) != null)
        );
    }
}
