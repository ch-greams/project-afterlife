using System.Collections;
using System.Collections.Generic;


// NOTE: Proc(?) trigger manager
public class EventTriggerManager : IWithEndOfTurnAction
{
    public List<EndOfTurnAction> endOfTurnActions { get { return this._endOfTurnActions; } }
    public List<EndOfTurnAction> _endOfTurnActions = new List<EndOfTurnAction>();

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        foreach (EndOfTurnAction endOfTurnAction in this.endOfTurnActions)
        {
            endOfTurnAction.Init(this.globalCtrl.globalState.endOfTurnActionState);   
        }
    }

    public IEnumerator OnTurnChange()
    {
        yield return null;
    }
}
