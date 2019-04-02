using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class EndOfTurnActionManager : SerializedMonoBehaviour
{
    public Dictionary<PlayerActionType, EndOfTurnActionList> actionLists = new Dictionary<PlayerActionType, EndOfTurnActionList>();

    private bool isInProgress = false;
    private bool skipActions = false;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        foreach (EndOfTurnActionList actionList in this.actionLists.Values)
        {
            actionList.Init(this.globalCtrl);
        }
    }


    public IEnumerator TriggerValidActions()
    {
        PlayerActionType actionType = this.globalCtrl.playerActionManager.currentAction;

        this.isInProgress = true;

        foreach (EndOfTurnAction action in this.actionLists[actionType].actions)
        {
            if (action.IsValid())
            {
                yield return action.React();
            }

            if (this.skipActions)
            {
                break;
            }
        }

        this.isInProgress = false;
        this.skipActions = false;
    }

    public void TrySkipActions()
    {
        if (this.isInProgress)
        {
            this.skipActions = true;
        }
    }
}
