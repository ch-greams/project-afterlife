using System.Collections;


public class VisibilityReaction : IReaction
{
    public int visibility = 2;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }
    public IEnumerator React()
    {
        this.sceneCtrl.globalCtrl.UpdatePlayerVisibility(this.visibility);
        this.sceneCtrl.player.visibleRange = this.visibility;
        this.sceneCtrl.UpdateTiles(this.sceneCtrl.player.tile);

        yield return null;
    }
}
