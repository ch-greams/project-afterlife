using System.Collections;


public class LightSourceReaction : IInteractableReaction
{
    public LightSourceReactionType type;
    private LightSourceData data;
    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.data = interactable.data as LightSourceData;
        this.sceneCtrl = interactable.sceneCtrl;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case LightSourceReactionType.ENABLE:
                this.sceneCtrl.sceneState.lightSources[this.data.id] = true;
                this.sceneCtrl.highlightedTiles[this.data.id] = this.data.highlightedTiles;
                this.sceneCtrl.UpdateHighlightedTiles(this.data.id);
                break;
            case LightSourceReactionType.DISABLE:
                this.sceneCtrl.sceneState.lightSources[this.data.id] = false;
                this.sceneCtrl.highlightedTiles.Remove(this.data.id);
                if (!this.sceneCtrl.sceneState.visibleByDefault)
                {
                    this.sceneCtrl.UpdateTiles(this.sceneCtrl.player.tile);
                }
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum LightSourceReactionType
{
    ENABLE,
    DISABLE,
}
