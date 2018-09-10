using System.Collections;


public class VisibilityReaction : IReaction
{
    public float visibility = 2.5F;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }
    
    public IEnumerator React()
    {
        this.sceneCtrl.globalCtrl.UpdatePlayerVisibility(this.visibility);
        this.sceneCtrl.player.visibleRange = this.visibility;

        if (!this.sceneCtrl.sceneState.visibleByDefault)
        {
            this.sceneCtrl.UpdateTiles(this.sceneCtrl.player.tile);
        }

        yield return null;
    }
}
