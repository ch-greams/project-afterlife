using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class PlayerReaction : IInteractableReaction
{
    public PlayerReactionType type;

    [ShowIf("type", PlayerReactionType.SetWalkableAreas), ValueDropdown("sceneNames")]
    public string sceneName;

    [ShowIf("type", PlayerReactionType.SetWalkableAreas)]
    public WalkableAreaMask walkableAreas;


    private SceneController sceneCtrl;
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }

    public IEnumerator React()
    {
        if (this.type == PlayerReactionType.SetWalkableAreas)
        {
            GlobalState globalState = this.sceneCtrl.globalCtrl.globalState;
            globalState.sceneStates[this.sceneName].walkableAreas = this.walkableAreas;

            if (this.sceneName == globalState.currentScene)
            {
                this.sceneCtrl.player.navMeshAgent.areaMask = (int)this.walkableAreas;
            }
        }

        yield return null;
    }
}


public enum PlayerReactionType
{
    SetWalkableAreas,
}
