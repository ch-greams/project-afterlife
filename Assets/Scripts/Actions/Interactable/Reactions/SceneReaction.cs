using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class SceneReaction : IInteractableReaction
{
    [ValueDropdown("sceneNames")]
    public string sceneName;
    public Point startPoint;

    private GlobalController globalCtrl;
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }

    public IEnumerator React()
    {
        this.globalCtrl.UpdatePlayerPosition(this.sceneName, this.startPoint);
        SceneManager.FadeAndLoadScene(this.sceneName);

        yield return null;
    }
}
