using System.Collections;


public class SceneReaction : IReaction
{
    public SceneType scene;
    public Point startPoint;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }
    public IEnumerator React()
    {
        // Update start point in state
        this.sceneCtrl.scene.UpdateStartPoint(this.scene, this.startPoint);
        // Switch scene
        SceneManager.FadeAndLoadScene(this.scene.ToString());

        yield return null;
    }
}
