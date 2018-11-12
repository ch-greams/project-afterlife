using System.Collections;
using UnityEngine;


public class TestDebugReaction : IEndOfTurnReaction
{
    public string logText = "Triggered";
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        Debug.Log("TestDebugReaction.React(): " + this.logText);

        yield return null;
    }
}
