using System.Collections;


public class SceneReaction : IReaction
{
    public SceneType scene;
    public Point startPoint;

    private GlobalController globalCtrl;


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }
    public IEnumerator React()
    {
        // Update start point in state
        this.globalCtrl.UpdatePlayerPosition(this.scene, this.startPoint);
        // Switch scene
        SceneManager.FadeAndLoadScene(this.scene.ToString());

        yield return null;
    }
}
