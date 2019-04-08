using System.Collections;
using UnityEngine;


public class TestDebugReaction : ITurnActionReaction, IInteractableReaction, IEnemyActionReaction
{
    public string logText = "Triggered";
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }

    public IEnumerator React()
    {
        Debug.Log("TestDebugReaction.React(): " + this.logText);

        yield return null;
    }
}
